using REIstacks.Domain.Entities.Properties;

namespace REIstacks.Domain.Entities.CRM;
public class PropertyBoard
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public int BoardId { get; set; }
    public int? PhaseId { get; set; }
    public DateTime AddedAt { get; set; }

    // Navigation properties
    public Property Property { get; set; }
    public Board Board { get; set; }
    public BoardPhase Phase { get; set; }
}