using System.Net;
using System.Net.Mail;
using Contracts;

namespace App.BLL;

public class EmailService : IEmailService
{
    public async Task SendEmailAsync(string uniId, string fullName, string oneTimeKey)
    {
        var mail = Environment.GetEnvironmentVariable("MAILSENDER_EMAIL") ?? "EMAIL";
        var key = Environment.GetEnvironmentVariable("MAILSENDER_KEY") ?? "KEY";
        var host = Environment.GetEnvironmentVariable("MAILSENDER_HOST") ?? "HOST";
        var port = int.Parse(Environment.GetEnvironmentVariable("MAILSENDER_PORT") ?? "0");

        var email = $"{uniId}@taltech.ee";
        var subject = $"{uniId} account verification";
        var messageBody = $"Hello, {uniId}! \n\nHere is a one-time key to verify ownership of Your EduCode account: {oneTimeKey}";
        
        var smtpClient = new SmtpClient(host)
        {
            Port = port,
            Credentials = new NetworkCredential(mail, key),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(mail),
            Subject = subject,
            Body = messageBody,
            IsBodyHtml = false
        };

        mailMessage.To.Add($"{fullName} <{email}>");

        try
        {
            await smtpClient.SendMailAsync(mailMessage);
        }
        catch (SmtpException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}