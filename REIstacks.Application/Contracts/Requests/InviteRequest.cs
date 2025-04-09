using System.ComponentModel.DataAnnotations;

namespace REIstacks.Application.Contracts.Requests;



public class InviteRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [StringLength(50)]
    public String Role { get; set; }  // The role the invitee will have (e.g., "VA", "Manager")
}

