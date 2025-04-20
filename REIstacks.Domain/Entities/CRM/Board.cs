using REIstacks.Domain.Entities.Organizations;

namespace REIstacks.Domain.Entities.CRM;
public class Board
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string OrganizationId { get; set; }
    public bool IsDefault { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Organization Organization { get; set; }
    public ICollection<BoardPhase> Phases { get; set; } = new List<BoardPhase>();
    public ICollection<PropertyBoard> PropertyBoards { get; set; } = new List<PropertyBoard>();
}