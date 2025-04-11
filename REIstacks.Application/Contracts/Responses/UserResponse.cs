

using REIstacks.Domain.Entities.Organizations;

namespace REIstacks.Application.Contracts.Responses;

public class UserResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public Role Role { get; set; } // This will now use your Role enum
    public string AvatarUrl { get; set; }
    public OrganizationResponse Organization { get; set; }
}