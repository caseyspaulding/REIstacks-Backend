using Microsoft.AspNetCore.Authentication;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using REIstacks.Application.Contracts.Requests;
using REIstacks.Application.Contracts.Responses;
using REIstacks.Application.Interfaces;
using REIstacks.Application.Repositories.Interfaces;
using REIstacks.Application.Services.Interfaces;
using REIstacks.Domain.Entities.Organizations;
using REIstacks.Domain.Entities.User;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace reistacks_api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IGoogleAuthService _googleAuthService;
        private readonly ITokenService _jwtService;
        private readonly IActivityLogger _activityLogger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(
            IGoogleAuthService googleAuthService,
            ITokenService jwtService,
            IActivityLogger activityLogger,
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            ILogger<AuthController> logger,
            IHttpClientFactory httpClientFactory,
            IEmailService emailService)
        {
            _googleAuthService = googleAuthService;
            _jwtService = jwtService;
            _activityLogger = activityLogger;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _emailService = emailService;
        }


        [HttpPost("google/callback")]
        public async Task<IActionResult> GoogleCallback([FromBody] GoogleCallbackRequest request)
        {
            if (string.IsNullOrEmpty(request.Code))
            {
                return BadRequest("Authorization code is required");
            }
            try
            {
                var tokenResponse = await ExchangeCodeForTokensAsync(request.Code);
                var userInfo = await GetGoogleUserInfoAsync(tokenResponse.AccessToken);

                // Create claims for authentication
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userInfo.Sub),
            new Claim(ClaimTypes.Name, userInfo.Name),
            new Claim(ClaimTypes.Email, userInfo.Email),
            new Claim("picture", userInfo.Picture)
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    claimsPrincipal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddDays(7)
                    });

                // Set the authentication cookies explicitly
                Response.Cookies.Append("access_token", tokenResponse.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Domain = ".reistacks.com",
                    Expires = DateTime.UtcNow.AddMinutes(30)
                });

                Response.Cookies.Append("refresh_token", tokenResponse.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Domain = ".reistacks.com",
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                // Check if user exists and needs setup using unit of work pattern
                var user = await _unitOfWork.UserProfiles.GetByExternalIdAsync(userInfo.Sub);
                bool requiresSetup = false;

                if (user == null)
                {
                    // First-time user, create a basic profile that needs setup
                    user = new UserProfile
                    {
                        Id = Guid.NewGuid(),
                        ExternalId = userInfo.Sub,
                        ExternalProvider = "Google",
                        Email = userInfo.Email,
                        Name = userInfo.Name,
                        IsSetupComplete = false,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.UserProfiles.AddAsync(user);
                    await _unitOfWork.CompleteAsync();
                    requiresSetup = true;
                }
                else
                {
                    // Existing user, check if setup is complete
                    requiresSetup = !user.IsSetupComplete;
                }

                // Return profile and setup status
                return Ok(new
                {
                    profile = new
                    {
                        id = user.Id.ToString(),
                        sub = userInfo.Sub,
                        name = userInfo.Name,
                        email = userInfo.Email,
                        picture = userInfo.Picture,
                        isSetupComplete = user.IsSetupComplete
                    },
                    requiresSetup = requiresSetup
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("current-user")]
        [Authorize] //  Requires authentication via secure cookies
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                if (User.Identity?.IsAuthenticated != true)
                {
                    Console.WriteLine("User not authenticated");
                    return Unauthorized(new { error = "User not authenticated" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var email = User.FindFirstValue(ClaimTypes.Email);
                var name = User.FindFirstValue(ClaimTypes.Name);
                var picture = User.FindFirstValue("picture");

                //  Await the user profile fetch from the database
                var user = await this._unitOfWork.UserProfiles.GetByIdWithOrganizationAsync(Guid.Parse(userId));

                if (user == null)
                {
                    return Unauthorized(new { error = "User not found" });
                }

                // Return user data, including setup status
                return Ok(new
                {
                    id = userId,
                    sub = userId,
                    name = name,
                    email = email,
                    role = user.Role.ToString(),
                    avatar = user.AvatarUrl,
                    organizationId = user.OrganizationId,
                    organizationName = user.Organization?.Name,
                    organizationLogoUrl = user.Organization?.LogoUrl,
                    organizationThemeColor = user.Organization?.ThemeColor,
                    subdomain = user.Organization?.Subdomain,
                    customDomain = user.Organization?.CustomDomain,
                    createdAt = user.CreatedAt,
                    updatedAt = user.UpdatedAt,
                    lastLogin = user.LastLogin,
                    isSetupComplete = user.IsSetupComplete
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpGet("status")]
        [EnableCors("ReistacksFrontend")]
        [AllowAnonymous]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                status = "ok",
                timestamp = DateTime.UtcNow,
                message = "API is reachable and CORS is configured correctly"
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            //  Remove authentication cookies
            Response.Cookies.Delete("access_token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Domain = ".reistacks.com"
            });

            Response.Cookies.Delete("refresh_token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Domain = ".reistacks.com"
            });

            return Ok(new { success = true });
        }

        private async Task<GoogleTokenResponse> ExchangeCodeForTokensAsync(string code)
        {
            var tokenRequestParameters = new Dictionary<string, string>
            {
                ["code"] = code,
                ["client_id"] = _configuration["AUTHENTICATION_GOOGLE_CLIENTID"],
                ["client_secret"] = _configuration["AUTHENTICATION_GOOGLE_CLIENTSECRET"],
                ["redirect_uri"] = "https://www.reistacks.com/api/auth/google/callback",
                ["grant_type"] = "authorization_code"
            };

            var tokenRequest = new FormUrlEncodedContent(tokenRequestParameters);
            var tokenResponse = await _httpClient.PostAsync("https://oauth2.googleapis.com/token", tokenRequest);

            if (!tokenResponse.IsSuccessStatusCode)
            {
                var errorContent = await tokenResponse.Content.ReadAsStringAsync();
                throw new Exception($"Failed to exchange code for tokens: {errorContent}");
            }

            return await tokenResponse.Content.ReadFromJsonAsync<GoogleTokenResponse>();
        }

        private async Task<GoogleUserInfo> GetGoogleUserInfoAsync(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            var userInfoResponse = await _httpClient.GetAsync("https://www.googleapis.com/oauth2/v3/userinfo");

            if (!userInfoResponse.IsSuccessStatusCode)
            {
                var errorContent = await userInfoResponse.Content.ReadAsStringAsync();
                throw new Exception($"Failed to fetch user info: {errorContent}");
            }

            return await userInfoResponse.Content.ReadFromJsonAsync<GoogleUserInfo>();
        }

        [HttpPost("google")]
        public async Task<IActionResult> GoogleSignIn([FromBody] GoogleLoginRequest request)
        {
            try
            {
                var validationResult = await _googleAuthService.ValidateGoogleTokenAsync(request.IdToken, request.Nonce);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new { error = "Invalid Google token" });
                }

                var email = validationResult.Claims["email"].ToString();

                var existingProfile = await _unitOfWork.UserProfiles.GetByEmailAsync(email);
                if (existingProfile != null)
                {
                    // Get the organization for the user
                    var organization = await _unitOfWork.Organizations.GetByIdAsync(existingProfile.OrganizationId);

                    if (existingProfile == null)
                    {
                        return Ok(new
                        {
                            requiresSetup = true,
                            email,
                            name = validationResult.Claims.ContainsKey("name") ? validationResult.Claims["name"].ToString() : null,
                            googleId = validationResult.Claims["sub"].ToString()
                        });
                    }

                    var profile = existingProfile;

                    var accessToken = _jwtService.GenerateAccessToken(profile, organization);
                    var refreshToken = _jwtService.GenerateRefreshToken();

                    await _jwtService.SaveRefreshTokenAsync(
                        profile.Id,
                        refreshToken,
                        HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        Request.Headers["User-Agent"].ToString());

                    await _activityLogger.LogActivityAsync(profile.OrganizationId, profile.Id, ActivityType.SignIn);
                    // Set cookies first
                    Response.Cookies.Append("access_token", accessToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Lax,
                        Domain = ".reistacks.com",
                        Expires = DateTime.UtcNow.AddMinutes(30)
                    });

                    Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Lax,
                        Domain = ".reistacks.com",
                        Expires = DateTime.UtcNow.AddDays(7)
                    });
                    return Ok(new
                    {
                        accessToken,
                        refreshToken,
                        profile = new
                        {
                            id = profile.Id,
                            email = profile.Email,
                            name = profile.Name,
                            role = profile.Role.ToString(),
                            isSetupComplete = profile.IsSetupComplete,
                            organizationId = profile.OrganizationId,
                            subdomain = organization.Subdomain
                        }


                    });

                }
                else
                {
                    return Ok(new
                    {
                        requiresSetup = true,
                        email,
                        name = validationResult.Claims.ContainsKey("name") ? validationResult.Claims["name"].ToString() : null,
                        googleId = validationResult.Claims["sub"].ToString()
                    });

                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Internal server error",
                    message = ex.Message,
                    stackTrace = ex.StackTrace,
                    innerException = ex.InnerException?.Message
                });
            }
        }

        [HttpPost("token-exchange")]
        public async Task<IActionResult> ExchangeCodeForTokens([FromBody] TokenRequest request)
        {
            try
            {

                // Step 1: Exchange Google OAuth code for tokens
                var googleTokens = await _googleAuthService.ExchangeCodeAsync(request.Code, request.CodeVerifier, request.RedirectUri);

                // Step 2: Validate Google's id_token (JWT)
                var validationResult = await _googleAuthService.ValidateGoogleTokenAsync(googleTokens.IdToken, request.Nonce);

                if (!validationResult.IsValid)
                {
                    return BadRequest(new { error = "Invalid Google ID token" });
                }

                var email = validationResult.Claims["email"].ToString();
                var name = validationResult.Claims.ContainsKey("name") ? validationResult.Claims["name"].ToString() : email;
                var googleId = validationResult.Claims["sub"].ToString();
                var picture = validationResult.Claims.ContainsKey("picture") ? validationResult.Claims["picture"].ToString() : "https://www.reistacks.com/default-avatar.png";
                Console.WriteLine($"Google ID: {googleId}, Email: {email}, Name: {name}, Picture: {picture}");
                // Check if user already exists
                var existingProfile = await _unitOfWork.UserProfiles.GetByEmailAsync(email);

                if (existingProfile == null)
                {
                    // User is new → Create profile
                    var profile = new UserProfile
                    {
                        Id = Guid.NewGuid(),
                        Email = email,
                        Name = name,
                        ExternalId = googleId,
                        IsSetupComplete = false,
                        OrganizationId = null,
                        Role = Role.Owner,
                        CreatedAt = DateTime.UtcNow,
                        AvatarUrl = picture
                    };

                    await _unitOfWork.UserProfiles.AddAsync(profile);
                    await _unitOfWork.CompleteAsync();

                    var newAccessToken = _jwtService.GenerateAccessToken(profile, null);
                    var newRefreshToken = _jwtService.GenerateRefreshToken();

                    await _jwtService.SaveRefreshTokenAsync(profile.Id, newRefreshToken,
                        HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        Request.Headers["User-Agent"].ToString());
                    Console.WriteLine($"New user profile created: {profile.Id}");
                    // Set cookies securely (new user)
                    Response.Cookies.Append("access_token", newAccessToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Lax,
                        Domain = ".reistacks.com",
                        Expires = DateTime.UtcNow.AddMinutes(30)
                    });

                    Response.Cookies.Append("refresh_token", newRefreshToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Lax,
                        Domain = ".reistacks.com",
                        Expires = DateTime.UtcNow.AddDays(7)
                    });
                    Console.WriteLine($"New user profile created: {profile.Id}");
                    return Ok(new
                    {
                        profile = new
                        {
                            id = profile.Id,
                            email = profile.Email,
                            name = profile.Name,
                            role = profile.Role.ToString(),
                            isSetupComplete = false,
                            requiresSetup = true
                        }
                    });
                }

                // Existing user scenario
                var organization = await _unitOfWork.Organizations.GetByIdAsync(existingProfile.OrganizationId);

                var accessToken = _jwtService.GenerateAccessToken(existingProfile, organization);
                var refreshToken = _jwtService.GenerateRefreshToken();

                await _jwtService.SaveRefreshTokenAsync(existingProfile.Id, refreshToken,
                    HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    Request.Headers["User-Agent"].ToString());

                await _activityLogger.LogActivityAsync(existingProfile.OrganizationId, existingProfile.Id, ActivityType.SignIn);
                Console.WriteLine($"Existing user signed in: {existingProfile.Id}");
                // Set cookies securely (existing user)
                Response.Cookies.Append("access_token", accessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Domain = ".reistacks.com",
                    Expires = DateTime.UtcNow.AddMinutes(30)
                });

                Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Domain = ".reistacks.com",
                    Expires = DateTime.UtcNow.AddDays(7)
                });
                Console.WriteLine($"Existing user signed in: {existingProfile.Id}");
                return Ok(new
                {
                    profile = new
                    {
                        id = existingProfile.Id,
                        email = existingProfile.Email,
                        name = existingProfile.Name,
                        role = existingProfile.Role.ToString(),
                        isSetupComplete = existingProfile.IsSetupComplete,
                        organizationId = existingProfile.OrganizationId,
                        subdomain = organization?.Subdomain,
                        requiresSetup = !existingProfile.IsSetupComplete
                    }
                });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new
                {

                    error = "Internal server error",
                    message = ex.Message,
                    stackTrace = ex.StackTrace,
                    innerException = ex.InnerException?.Message

                });

            }
        }

        [HttpPost("setup")]
        [Authorize]
        [EnableCors("ReistacksFrontend")]
        public async Task<IActionResult> Setup([FromBody] OrganizationSetupRequest request)
        {
            // Log request to help with debugging
            Console.WriteLine($"Setup request received: {JsonSerializer.Serialize(request)}");
            Console.WriteLine($"Auth header: {Request.Headers["Authorization"]}");
            Console.WriteLine($"Origin: {Request.Headers["Origin"]}");

            // Explicitly add CORS headers for this endpoint
            Response.Headers.Add("Access-Control-Allow-Origin", Request.Headers["Origin"].ToString());
            Response.Headers.Add("Access-Control-Allow-Credentials", "true");

            try
            {
                // Get the current user from the JWT token
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Console.WriteLine($"User ID from token: {userId}");

                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User ID not found in token");
                    return Unauthorized(new { error = "User not found in token" });
                }

                if (!Guid.TryParse(userId, out Guid profileId))
                {
                    Console.WriteLine($"Profile not found for user ID: {userId}");
                    return BadRequest(new { error = "User profile not found" });
                }

                var profile = await _unitOfWork.UserProfiles.GetByIdAsync(userId);

                if (profile == null)
                {
                    Console.WriteLine($"Profile not found for user ID: {userId}");
                    return BadRequest(new { error = "User profile not found" });
                }

                var organizationId = Guid.NewGuid().ToString();
                var organization = new Organization
                {
                    Id = organizationId,
                    Name = request.OrganizationName,
                    Subdomain = NormalizeSubdomain(request.Subdomain),
                    CustomDomain = $"pending-setup-{organizationId}",
                    OwnerId = profile.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Validate subdomain uniqueness
                if (await _unitOfWork.Organizations.SubdomainExistsAsync(organization.Subdomain))
                    return BadRequest(new { error = "Subdomain is already taken" });

                await _unitOfWork.Organizations.AddAsync(organization);

                // Update Profile
                profile.OrganizationId = organizationId;
                profile.Name = request.Name;
                profile.Role = Role.Owner;
                profile.IsSetupComplete = true;
                profile.SmsConsent = request.SmsConsent;
                profile.SmsConsentDate = request.SmsConsent ? DateTime.UtcNow : null;
                _unitOfWork.UserProfiles.Update(profile);
                await _unitOfWork.CompleteAsync();

                // Remove leads processing section since we no longer need it

                // Generate tokens
                var accessToken = _jwtService.GenerateAccessToken(profile, organization);
                var refreshToken = _jwtService.GenerateRefreshToken();

                // Save refresh token
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var userAgent = Request.Headers["User-Agent"].ToString();

                await _jwtService.SaveRefreshTokenAsync(profile.Id, refreshToken, ipAddress, userAgent);

                // Set tokens as secure cookies only
                Response.Cookies.Append("access_token", accessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Domain = ".reistacks.com",
                    Expires = DateTime.UtcNow.AddMinutes(30)
                });

                Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Domain = ".reistacks.com",
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                await _unitOfWork.CompleteAsync();

                await _activityLogger.LogActivityAsync(
                    organizationId,
                    profile.Id,
                    ActivityType.CreateTeam,
                    HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown");

                return Ok(new
                {
                    profile = new
                    {
                        id = profile.Id,
                        email = profile.Email,
                        name = profile.Name,
                        role = profile.Role.ToString(),
                        isSetupComplete = profile.IsSetupComplete,
                        organizationId = profile.OrganizationId,
                        subdomain = organization.Subdomain
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Setup error: {ex.Message}");
                return StatusCode(500, new
                {
                    error = "Internal server error",
                    message = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }


        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                // ✅ Read refresh token from cookies instead of request body
                var refreshToken = Request.Cookies["refresh_token"];

                if (string.IsNullOrEmpty(refreshToken))
                    return Unauthorized(new { error = "Refresh token missing" });

                var storedRefreshToken = await _jwtService.GetRefreshTokenAsync(refreshToken);
                if (storedRefreshToken == null || storedRefreshToken.ExpiryDate < DateTime.UtcNow)
                    return Unauthorized(new { error = "Invalid or expired refresh token" });

                var profile = await _unitOfWork.UserProfiles.GetByIdAsync(storedRefreshToken.ProfileId);
                if (profile == null)
                    return Unauthorized(new { error = "User not found" });

                var organization = await _unitOfWork.Organizations.GetByIdAsync(profile.OrganizationId);
                if (organization == null)
                    return Unauthorized(new { error = "Organization not found" });

                // Generate new tokens
                var accessToken = _jwtService.GenerateAccessToken(profile, organization);
                var newRefreshToken = _jwtService.GenerateRefreshToken();

                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                string userAgent = Request.Headers["User-Agent"].ToString();

                // ✅ Remove old refresh token and save the new one
                await _jwtService.RevokeRefreshTokenAsync(refreshToken);
                await _jwtService.SaveRefreshTokenAsync(profile.Id, newRefreshToken, ipAddress, userAgent);

                // ✅ Set tokens securely in HttpOnly cookies
                Response.Cookies.Append("access_token", accessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Domain = ".reistacks.com",
                    Expires = DateTime.UtcNow.AddMinutes(30)
                });

                Response.Cookies.Append("refresh_token", newRefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Domain = ".reistacks.com",
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpGet("test-auth")]
        [AllowAnonymous]
        public IActionResult TestAuth()
        {
            // Check existing cookies
            var accessTokenCookie = Request.Cookies["access_token"];
            var refreshTokenCookie = Request.Cookies["refresh_token"];

            // Create test JWT token with claims we can verify
            var testClaims = new List<Claim> {
        new Claim("test_claim", "test_value"),
        new Claim(ClaimTypes.Name, "Test User")
    };

            var testAccessToken = "test_token_" + Guid.NewGuid().ToString();
            var testRefreshToken = "test_refresh_" + Guid.NewGuid().ToString();

            // Set the cookies
            Response.Cookies.Append("test_access_token", testAccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Domain = ".reistacks.com",
                Expires = DateTime.UtcNow.AddMinutes(30)
            });

            Response.Cookies.Append("test_refresh_token", testRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Domain = ".reistacks.com",
                Expires = DateTime.UtcNow.AddDays(7)
            });

            // Return diagnostic information
            return Ok(new
            {
                existingCookies = new
                {
                    hasAccessToken = !string.IsNullOrEmpty(accessTokenCookie),
                    hasRefreshToken = !string.IsNullOrEmpty(refreshTokenCookie),
                    accessTokenLength = accessTokenCookie?.Length ?? 0,
                    refreshTokenLength = refreshTokenCookie?.Length ?? 0
                },
                testCookiesSet = new
                {
                    testAccessToken,
                    testRefreshToken
                },
                cookieSettings = new
                {
                    domain = ".reistacks.com",
                    sameSite = "Lax",
                    secure = true,
                    httpOnly = true
                },
                userAuth = new
                {
                    isAuthenticated = User.Identity?.IsAuthenticated ?? false,
                    claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
                },
                serverInfo = new
                {
                    serverTime = DateTime.UtcNow,
                    requestPath = Request.Path,
                    requestMethod = Request.Method,
                    requestHeaders = Request.Headers.Select(h => new { h.Key, Value = h.Value.ToString() }).ToList()
                }
            });
        }

        [HttpGet("verify-test-auth")]
        [AllowAnonymous]
        public IActionResult VerifyTestAuth()
        {
            // Check for test cookies
            var testAccessToken = Request.Cookies["test_access_token"];
            var testRefreshToken = Request.Cookies["test_refresh_token"];

            // Regular auth cookies
            var accessToken = Request.Cookies["access_token"];
            var refreshToken = Request.Cookies["refresh_token"];

            return Ok(new
            {
                testCookies = new
                {
                    hasTestAccessToken = !string.IsNullOrEmpty(testAccessToken),
                    hasTestRefreshToken = !string.IsNullOrEmpty(testRefreshToken),
                    testAccessToken,
                    testRefreshToken
                },
                regularCookies = new
                {
                    hasAccessToken = !string.IsNullOrEmpty(accessToken),
                    hasRefreshToken = !string.IsNullOrEmpty(refreshToken)
                },
                requestInfo = new
                {
                    path = Request.Path,
                    method = Request.Method,
                    origin = Request.Headers["Origin"].ToString(),
                    referer = Request.Headers["Referer"].ToString()
                }
            });
        }

        private string NormalizeSubdomain(string subdomain)
        {
            // Remove spaces, special characters, convert to lowercase
            return new string(subdomain.ToLower()
                .Where(c => char.IsLetterOrDigit(c) || c == '-')
                .ToArray())
                .Trim('-'); // Remove leading/trailing hyphens
        }


        [HttpGet("test-cookies")]
        [AllowAnonymous]
        [EnableCors("ReistacksFrontend")]
        public IActionResult TestCookies()
        {
            // Get existing cookies
            var existingCookies = Request.Cookies.Select(c => new
            {
                c.Key,
                ValuePreview = c.Value.Length > 10 ? c.Value.Substring(0, 10) + "..." : c.Value,
                Length = c.Value.Length
            }).ToList();

            // Set test cookies with different SameSite options to see which one works
            Response.Cookies.Append("test_cookie_none", "test_value_none", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Domain = ".reistacks.com",
                Expires = DateTime.UtcNow.AddMinutes(5)
            });

            Response.Cookies.Append("test_cookie_lax", "test_value_lax", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Domain = ".reistacks.com",
                Expires = DateTime.UtcNow.AddMinutes(5)
            });

            // Also set a non-HttpOnly cookie that JavaScript can see
            Response.Cookies.Append("js_visible_cookie", "js_can_see_this", new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.None,
                Domain = ".reistacks.com",
                Expires = DateTime.UtcNow.AddMinutes(5)
            });

            // Return information about existing cookies and what we set
            return Ok(new
            {
                existingCookies,
                cookiesSet = new[] {
            "test_cookie_none (SameSite=None)",
            "test_cookie_lax (SameSite=Lax)",
            "js_visible_cookie (HttpOnly=false)"
        },
                requestInfo = new
                {
                    host = Request.Host.ToString(),
                    path = Request.Path.ToString(),
                    userAgent = Request.Headers["User-Agent"].ToString(),
                    secure = Request.IsHttps,
                    origin = Request.Headers["Origin"].ToString(),
                    hasAuthHeader = !string.IsNullOrEmpty(Request.Headers["Authorization"].ToString())
                }
            });
        }

        [HttpPost("invite")]
        [Authorize]
        public async Task<IActionResult> InviteUser([FromBody] InviteRequest request)
        {
            try
            {
                _logger.LogInformation("InviteUser called with email: {Email}, role: {Role}", request.Email, request.Role);

                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Role))
                {
                    _logger.LogWarning("Invalid request parameters: Email or Role is empty");
                    return BadRequest("Email and role are required");
                }

                var inviterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(inviterId))
                {
                    _logger.LogWarning("User identifier not found in claims");
                    return Unauthorized("User identity not found");
                }

                _logger.LogDebug("Fetching profile for inviter ID: {InviterId}", inviterId);
                var inviterProfile = await _unitOfWork.UserProfiles.GetByIdAsync(Guid.Parse(inviterId));
                if (inviterProfile == null)
                {
                    _logger.LogWarning("Profile not found for inviter ID: {InviterId}", inviterId);
                    return Unauthorized("Invalid user");
                }

                // Convert string to enum - ADDED ignoreCase parameter to fix case sensitivity issue
                if (!Enum.TryParse(request.Role, ignoreCase: true, out Role roleEnum))
                {
                    _logger.LogWarning("Invalid role specified: {Role}", request.Role);
                    var availableRoles = string.Join(", ", Enum.GetNames(typeof(Role)));
                    _logger.LogInformation("Available roles: {Roles}", availableRoles);
                    return BadRequest($"Invalid role specified. Available roles: {availableRoles}");
                }

                var invite = new Invitation
                {
                    OrganizationId = inviterProfile.OrganizationId,
                    Email = request.Email,
                    Role = roleEnum,
                    InvitedBy = inviterProfile.Id,
                    Token = Guid.NewGuid().ToString(),
                    Status = "pending",
                    InvitedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddDays(7)
                };

                _logger.LogDebug("Creating invitation for email: {Email} to organization: {OrganizationId}",
                    request.Email, inviterProfile.OrganizationId);

                await _unitOfWork.Invitations.AddAsync(invite);
                await _unitOfWork.CompleteAsync();

                try
                {
                    await _emailService.SendInviteEmail(request.Email, invite.Token);
                    _logger.LogInformation("Invitation email sent successfully to: {Email}", request.Email);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send invitation email to: {Email}", request.Email);
                    return StatusCode(500, "Invitation created but failed to send email");
                }

                return Ok(new { message = "Invitation sent successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing invitation request");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost("accept-invite")]
        [Authorize]
        public async Task<IActionResult> AcceptInvite([FromBody] AcceptInviteRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.InviteToken))
                {
                    _logger.LogWarning("AcceptInvite called with empty token");
                    return BadRequest("Invite token is required");
                }

                _logger.LogInformation("AcceptInvite called with token: {Token}", request.InviteToken);

                var invite = await _unitOfWork.Invitations.GetByTokenAsync(request.InviteToken);
                if (invite == null)
                {
                    _logger.LogWarning("Invitation not found with token: {Token}", request.InviteToken);
                    return BadRequest("Invalid invite");
                }

                if (invite.Status != "pending")
                {
                    _logger.LogWarning(
                        "Attempted to accept non-pending invitation with token: {Token}, status: {Status}",
                        request.InviteToken, invite.Status
                    );
                    return BadRequest("Invitation has already been processed");
                }

                if (invite.ExpiresAt < DateTime.UtcNow)
                {
                    _logger.LogWarning(
                        "Attempted to accept expired invitation with token: {Token}, expired at: {ExpiryDate}",
                        request.InviteToken, invite.ExpiresAt
                    );
                    return BadRequest("Invitation has expired");
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User identifier not found in claims during invite acceptance");
                    return Unauthorized("User identity not found");
                }

                _logger.LogDebug("Fetching profile for user ID: {UserId}", userId);
                var userProfile = await _unitOfWork.UserProfiles.GetByIdAsync(Guid.Parse(userId));
                if (userProfile == null)
                {
                    _logger.LogWarning("Profile not found for user ID: {UserId}", userId);
                    return Unauthorized("User profile not found");
                }

                // Save original role if needed, or skip if not.
                var originalRole = invite.Role;

                _logger.LogDebug("Updating user {UserId} organization to: {OrganizationId}", userId, invite.OrganizationId);

                // Update user and invite
                userProfile.OrganizationId = invite.OrganizationId;
                userProfile.Role = invite.Role;   // or originalRole if you prefer
                userProfile.IsSetupComplete = true;
                userProfile.UpdatedAt = DateTime.UtcNow;

                invite.Role = originalRole;       // or remove if you're not overriding anything
                invite.Status = "accepted";
                invite.AcceptedAt = DateTime.UtcNow;
                invite.AcceptedByProfileId = userProfile.Id;
                invite.UpdatedAt = DateTime.UtcNow;

                // Persist changes
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation(
                    "Invitation {Token} successfully accepted by user {UserId}",
                    request.InviteToken, userId
                );

                // 🔑 Re-issue tokens so the user's new org/role are reflected in the JWT claims
                var updatedOrg = await _unitOfWork.Organizations.GetByIdAsync(userProfile.OrganizationId);
                _jwtService.SetAuthCookies(HttpContext, userProfile, updatedOrg);

                // Return success with a simple message, or additional data if needed
                return Ok(new { message = "Invite accepted" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing invitation acceptance");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        public class GoogleUserInfo
        {
            [JsonPropertyName("sub")]
            public string Sub { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("email")]
            public string Email { get; set; }

            [JsonPropertyName("picture")]
            public string Picture { get; set; }

            [JsonPropertyName("email_verified")]
            public bool EmailVerified { get; set; }
        }

    }
}