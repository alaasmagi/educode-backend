namespace WebApp.Models;

public class RequestOtpModel
{
    public required string Email { get; set; }
    public string? FullName { get; set; }
}