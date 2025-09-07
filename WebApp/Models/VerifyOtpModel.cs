namespace WebApp.Models;

public class VerifyOtpModel
{
    public required string Email { get; set; }
    public required string Otp { get; set; }
}