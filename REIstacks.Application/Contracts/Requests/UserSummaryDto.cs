namespace REIstacks.Application.Contracts.Requests;
public class UserSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? AvatarUrl { get; set; }
}
