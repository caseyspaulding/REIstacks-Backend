namespace REIstacks.Application.Services.Interfaces;

public interface IEmailService
{
    Task<bool> SendInviteEmail(string toEmail, string inviteId);
}
