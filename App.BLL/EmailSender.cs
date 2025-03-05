using System.Net;
using System.Net.Mail;
using App.Domain;

namespace App.BLL;

public class EmailSender
{
    public async Task SendEmail(UserEntity user, string oneTimeKey)
    {
        var mail = Environment.GetEnvironmentVariable("MAILSENDER_EMAIL") ?? "EMAIL";
        var key = Environment.GetEnvironmentVariable("MAILSENDER_KEY") ?? "KEY";
        var host = Environment.GetEnvironmentVariable("MAILSENDER_HOST") ?? "HOST";
        var port = int.Parse(Environment.GetEnvironmentVariable("MAILSENDER_PORT") ?? "0");

        var email = $"{user.UniId}@taltech.ee";
        var subject = $"{user.UniId} account verification";
        var messageBody = $"Hello, {user.FullName}! \n\nHere is a one-time key to verify ownership of Your EduCode account: {oneTimeKey}";
        
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

        mailMessage.To.Add($"{user.FullName} <{email}>");

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