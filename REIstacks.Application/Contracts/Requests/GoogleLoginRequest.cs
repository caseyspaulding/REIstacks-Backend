using System.ComponentModel.DataAnnotations;

namespace REIstacks.Application.Contracts.Requests;

public class GoogleLoginRequest
{
    [Required]
    public string IdToken { get; set; }

    public string Nonce { get; set; } = string.Empty;
}