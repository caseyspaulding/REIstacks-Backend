using Microsoft.AspNetCore.Http;
using REIstacks.Domain.Entities.Auth;
using REIstacks.Domain.Entities.Organizations;
using REIstacks.Domain.Entities.User;
using System.Security.Claims;

namespace REIstacks.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(UserProfile user, Organization org);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    Task SaveRefreshTokenAsync(Guid userId, string token, string ipAddress, string userAgent);
    Task<RefreshToken> GetRefreshTokenAsync(string token);
    Task RevokeRefreshTokenAsync(string token);
    void SetAuthCookies(HttpContext context, UserProfile user, Organization org);
    Task<bool> RefreshTokenAsync(HttpContext context, string refreshTokenFromCookie);
    void ClearAuthCookies(HttpContext context);
}