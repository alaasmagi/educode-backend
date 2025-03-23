namespace Contracts;

public interface IEmailService
{
    Task SendEmailAsync(string uniId, string fullName, string oneTimeKey);
}