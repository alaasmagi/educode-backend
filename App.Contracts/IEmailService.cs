namespace Contracts;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string uniId, string fullName, string oneTimeKey);
}