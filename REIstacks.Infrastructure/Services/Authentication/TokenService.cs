using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using REIstack.Domain.Models;
using REIstacks.Application.Interfaces;
using REIstacks.Application.Repositories.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace REIstacks.Infrastructure.Services.Authentication
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public TokenService(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        public string GenerateAccessToken(UserProfile user, Organization org)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT_SECRET"]));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Name, user.Name ?? string.Empty),
        new Claim(ClaimTypes.Role, user.Role.ToString())
    };

            // Add organization claims only if org is not null
            if (org != null)
            {
                claims.Add(new Claim("organization_id", org.Id));
                claims.Add(new Claim("subdomain", org.Subdomain));

                if (!string.IsNullOrEmpty(org.CustomDomain))
                {
                    claims.Add(new Claim("custom_domain", org.CustomDomain));
                }
            }
            else
            {
                // Add empty placeholder claims for consistency
                claims.Add(new Claim("organization_id", ""));
                claims.Add(new Claim("subdomain", ""));
            }

            var tokenOptions = new JwtSecurityToken(
                issuer: Constants.JwtConstants.Issuer,
                audience: Constants.JwtConstants.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT_SECRET"])),
                ValidIssuer = Constants.JwtConstants.Issuer,
                ValidAudience = Constants.JwtConstants.Audience,
                ValidateLifetime = false
            };
            Console.WriteLine("Token validation parameters set");

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            Console.WriteLine("Token validated successfully");
            return principal;
        }

        public async Task SaveRefreshTokenAsync(Guid userId, string token, string ipAddress, string userAgent)
        {
            var refreshToken = new RefreshToken
            {
                Token = token,
                ProfileId = userId,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IpAddress = ipAddress,
                UserAgent = userAgent,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<RefreshToken> GetRefreshTokenAsync(string token)
        {
            return await _unitOfWork.RefreshTokens.GetByTokenAsync(token);
        }

        public async Task RevokeRefreshTokenAsync(string token)
        {
            await _unitOfWork.RefreshTokens.RevokeTokenAsync(token);
            await _unitOfWork.CompleteAsync();
        }
        public void SetAuthCookies(HttpContext context, UserProfile user, Organization org)
        {
            // Generate tokens
            var accessToken = GenerateAccessToken(user, org);
            var refreshToken = GenerateRefreshToken();

            // Set access token cookie
            context.Response.Cookies.Append("access_token", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Domain = ".reistacks.com",
                Expires = DateTime.UtcNow.AddMinutes(30)
            });

            // Set refresh token cookie
            context.Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Domain = ".reistacks.com",
                Expires = DateTime.UtcNow.AddDays(7)
            });

            // Save refresh token to database
            _ = SaveRefreshTokenAsync(user.Id, refreshToken,
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                context.Request.Headers["User-Agent"].ToString() ?? "unknown");
        }

        public async Task<bool> RefreshTokenAsync(HttpContext context, string refreshTokenFromCookie)
        {
            try
            {
                // Get the refresh token from database
                var storedRefreshToken = await GetRefreshTokenAsync(refreshTokenFromCookie);

                if (storedRefreshToken == null || storedRefreshToken.IsRevoked ||
                    storedRefreshToken.ExpiryDate < DateTime.UtcNow)
                {
                    return false;
                }

                // Get user profile
                var userProfile = await _unitOfWork.UserProfiles.GetByIdAsync(storedRefreshToken.ProfileId);
                if (userProfile == null)
                {
                    return false;
                }

                // Get organization if exists
                Organization org = null;
                if (!string.IsNullOrEmpty(userProfile.OrganizationId))
                {
                    org = await _unitOfWork.Organizations.GetByIdAsync(userProfile.OrganizationId);
                }

                // Generate new tokens
                var newAccessToken = GenerateAccessToken(userProfile, org);
                var newRefreshToken = GenerateRefreshToken();

                // Revoke old refresh token
                await RevokeRefreshTokenAsync(refreshTokenFromCookie);

                // Set new cookies
                context.Response.Cookies.Append("access_token", newAccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Domain = ".reistacks.com",
                    Expires = DateTime.UtcNow.AddMinutes(30)
                });
                // Only set these in production

                context.Response.Cookies.Append("refresh_token", newRefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Domain = ".reistacks.com",
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                // Save new refresh token
                await SaveRefreshTokenAsync(userProfile.Id, newRefreshToken,
                    context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    context.Request.Headers["User-Agent"].ToString() ?? "unknown");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing token: {ex.Message}");
                return false;
            }
        }

        public void ClearAuthCookies(HttpContext context)
        {
            context.Response.Cookies.Delete("access_token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Domain = ".reistacks.com"
            });

            context.Response.Cookies.Delete("refresh_token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Domain = ".reistacks.com"
            });
        }


    }
}