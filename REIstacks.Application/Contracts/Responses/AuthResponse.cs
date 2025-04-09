namespace REIstacks.Application.Contracts.Responses;

public class AuthResponse
{
    public string Token { get; set; }
    public bool RequiresSetup { get; set; }
    public UserResponse User { get; set; }
}
