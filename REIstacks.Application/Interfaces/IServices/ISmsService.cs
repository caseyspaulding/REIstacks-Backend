namespace REIstacks.Application.Interfaces.IServices;
public interface ISmsService
{
    Task<bool> SendSmsAsync(string phoneNumber, string message);
}
