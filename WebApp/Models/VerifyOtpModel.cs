namespace WebApp.Models;

public class VerifyOtpModel
{
    public required string UniId { get; set; }
    public required string Otp { get; set; }
}