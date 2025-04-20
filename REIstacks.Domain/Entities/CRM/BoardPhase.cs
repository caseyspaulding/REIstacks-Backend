namespace REIstacks.Domain.Entities.CRM;
public class BoardPhase
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int BoardId { get; set; }
    public int DisplayOrder { get; set; }

    // Navigation properties
    public Board Board { get; set; }
    public ICollection<PropertyBoard> PropertyBoards { get; set; } = new List<PropertyBoard>();
}
