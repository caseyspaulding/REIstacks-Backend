namespace REIstacks.Application.Contracts.Requests;
public class SalesLeaderBoardRequest
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = "";
    public string? AvatarUrl { get; set; }
    public int CallsMade { get; set; }
    public int AppointmentsSet { get; set; }
    public int ContactsMade { get; set; }
}
