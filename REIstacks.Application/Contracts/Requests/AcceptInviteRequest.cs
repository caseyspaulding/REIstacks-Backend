using System.ComponentModel.DataAnnotations;

namespace REIstacks.Application.Contracts.Requests;


public class AcceptInviteRequest
{
    [Required]
    public string InviteToken { get; set; }
    public string Role { get; set; }
}

