using System.ComponentModel.DataAnnotations;

namespace REIstacks.Application.Contracts.Requests;

public class TeamJoinRequest
{
    [Required]
    public string IdToken { get; set; }

    [Required]
    public string InviteId { get; set; }

    public string Nonce { get; set; }
}