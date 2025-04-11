using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using REIstacks.Application.Contracts.Responses;
using REIstacks.Application.Repositories.Interfaces;
using REIstacks.Application.Services.Interfaces;
using REIstacks.Domain.Entities.Auth;
using REIstacks.Domain.Entities.Organizations;
using REIstacks.Domain.Entities.User;
using System.Net.Http.Json;
using System.Security.Claims;

namespace REIstacks.Infrastructure.Services.Authentication
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _clientId;

        public GoogleAuthService(
            IConfiguration configuration,
            IUnitOfWork unitOfWork,
            IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _httpClientFactory = httpClientFactory;
            _clientId = _configuration["AUTHENTICATION_GOOGLE_CLIENTID"];
        }

        public async Task<Microsoft.IdentityModel.Tokens.TokenValidationResult> ValidateGoogleTokenAsync(string token, string nonce)
        {
            // Token validation code remains the same
            const string GoogleOpenIdUrl = "https://accounts.google.com/.well-known/openid-configuration";
            var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                GoogleOpenIdUrl,
                new OpenIdConnectConfigurationRetriever(),
                new HttpDocumentRetriever());

            var googleConfig = await configManager.GetConfigurationAsync();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = googleConfig.Issuer,
                ValidateAudience = true,
                ValidAudience = _clientId,
                ValidateLifetime = true,
                IssuerSigningKeys = googleConfig.SigningKeys,
                ValidateIssuerSigningKey = true,
                NameClaimType = "name",
                RoleClaimType = ClaimTypes.Role
            };

            var handler = new JsonWebTokenHandler();
            var result = handler.ValidateToken(token, validationParameters);

            if (!string.IsNullOrEmpty(nonce))
            {
                var nonceFromToken = result.Claims.ContainsKey("nonce")
                    ? result.Claims["nonce"].ToString()
                    : null;

                if (nonceFromToken != nonce)
                {
                    throw new SecurityTokenException("Invalid nonce");
                }
            }

            return result;
        }
        public async Task<GoogleTokenResponse> ExchangeCodeAsync(string code, string verifier, string redirectUri)
        {
            var clientId = _configuration["AUTHENTICATION_GOOGLE_CLIENTID"];
            var clientSecret = _configuration["AUTHENTICATION_GOOGLE_CLIENTSECRET"];
            var googleTokenEndpoint = "https://oauth2.googleapis.com/token";

            var tokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>
    {
        {"code", code},
        {"client_id", clientId},
        {"client_secret", clientSecret},
        {"redirect_uri", redirectUri},
        {"grant_type", "authorization_code"},
        {"code_verifier", verifier}
    });

            var response = await _httpClientFactory.CreateClient().PostAsync(googleTokenEndpoint, tokenRequest);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Google token exchange failed: {errorContent}");
            }

            var tokenData = await response.Content.ReadFromJsonAsync<GoogleTokenResponse>();
            return tokenData;
        }


        public async Task<UserProfile> FindOrCreateProfileFromGoogleAuthAsync(
            Microsoft.IdentityModel.Tokens.TokenValidationResult validationResult,
            string role = null,
            string organizationId = null)
        {
            if (!validationResult.IsValid)
            {
                throw new SecurityTokenException("Invalid token");
            }

            var claims = validationResult.Claims;

            // Extract claims
            var googleId = claims["sub"].ToString();
            var email = claims["email"].ToString();
            var name = claims.ContainsKey("name") ? claims["name"].ToString() : email;
            var pictureUrl = claims.ContainsKey("picture") ? claims["picture"].ToString() : null;
            var emailVerified = claims.ContainsKey("email_verified") && bool.Parse(claims["email_verified"].ToString());
            var provider = "Google";
            // Use the repository to find existing external auth
            var existingAuth = await _unitOfWork.ExternalAuths.GetByExternalIdAsync(googleId, provider);

            if (existingAuth != null)
            {
                // Update profile info
                var profile = await _unitOfWork.UserProfiles.GetByIdAsync(existingAuth.ProfileId);

                profile.Name = name;
                profile.AvatarUrl = pictureUrl;
                profile.EmailVerifiedAt = emailVerified ? DateTime.UtcNow : profile.EmailVerifiedAt;
                profile.LastLogin = DateTime.UtcNow;
                profile.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.UserProfiles.Update(profile);
                await _unitOfWork.CompleteAsync();

                return profile;
            }

            // If no existing auth but organization ID provided, create new profile
            if (!string.IsNullOrEmpty(organizationId))
            {
                var organization = await _unitOfWork.Organizations.GetByIdAsync(organizationId);

                if (organization == null)
                {
                    throw new Exception("Organization not found");
                }

                // Find by email first (for invited users)
                var existingProfileByEmail = await _unitOfWork.UserProfiles.GetByEmailAndOrganizationAsync(email, organizationId);

                if (existingProfileByEmail != null)
                {
                    // Link existing profile to Google account
                    var newAuth = new ExternalAuth
                    {
                        Id = Guid.NewGuid(),
                        ProfileId = existingProfileByEmail.Id,
                        Provider = "Google",
                        ExternalId = googleId
                    };

                    await _unitOfWork.ExternalAuths.AddAsync(newAuth);

                    existingProfileByEmail.Name = name;
                    existingProfileByEmail.AvatarUrl = pictureUrl;
                    existingProfileByEmail.EmailVerifiedAt = emailVerified ? DateTime.UtcNow : null;
                    existingProfileByEmail.LastLogin = DateTime.UtcNow;
                    existingProfileByEmail.UpdatedAt = DateTime.UtcNow;

                    _unitOfWork.UserProfiles.Update(existingProfileByEmail);
                    await _unitOfWork.CompleteAsync();

                    return existingProfileByEmail;
                }

                // Create new profile and auth
                var newProfile = new UserProfile
                {
                    Id = Guid.NewGuid(),
                    Email = email,
                    Name = name,
                    AvatarUrl = pictureUrl,
                    EmailVerifiedAt = emailVerified ? DateTime.UtcNow : null,
                    OrganizationId = organizationId,
                    Role = string.IsNullOrEmpty(role) ? Role.Member : Enum.Parse<Role>(role),
                    LastLogin = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.UserProfiles.AddAsync(newProfile);

                var auth = new ExternalAuth
                {
                    Id = Guid.NewGuid(),
                    ProfileId = newProfile.Id,
                    Provider = "Google",
                    ExternalId = googleId
                };

                await _unitOfWork.ExternalAuths.AddAsync(auth);
                await _unitOfWork.CompleteAsync();

                return newProfile;
            }

            // No organization ID and no existing auth
            return null;
        }
    }
}