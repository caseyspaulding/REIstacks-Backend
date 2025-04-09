namespace REIstacks.Domain.Models.Configuration;

public class JwtSettings
{
    public string SecretName { get; set; } // AWS Secrets Manager secret name
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int ExpiryMinutes { get; set; }
    // This should hold the actual JWT signing key or an override from Secrets Manager
    public string Secret { get; set; }
}
