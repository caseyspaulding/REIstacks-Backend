using System.ComponentModel.DataAnnotations;

namespace REIstacks.Application.Contracts.Requests;
public class AssignLeadRequest
{
    [Required]
    public string ProfileId { get; set; }
}

