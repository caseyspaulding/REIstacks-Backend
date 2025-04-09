using Microsoft.IdentityModel.Tokens;
using REIstack.Domain.Models;
using REIstacks.Application.Contracts.Responses;


namespace REIstacks.Application.Services.Interfaces;
public interface IGoogleAuthService
{
    Task<GoogleTokenResponse> ExchangeCodeAsync(string code, string verifier, string redirectUri);
    Task<UserProfile> FindOrCreateProfileFromGoogleAuthAsync(TokenValidationResult validationResult, string role = null, string organizationId = null);
    Task<TokenValidationResult> ValidateGoogleTokenAsync(string token, string nonce);
}