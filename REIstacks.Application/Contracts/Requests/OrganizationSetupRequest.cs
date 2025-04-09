using System.ComponentModel.DataAnnotations;

namespace REIstacks.Application.Contracts.Requests;

public class OrganizationSetupRequest
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string OrganizationName { get; set; }

    [Required]
    public string Subdomain { get; set; }
    public bool SmsConsent { get; set; } = false;

    // Team members can be added but not required
    public List<string> TeamMembers { get; set; } = new List<string>();
}