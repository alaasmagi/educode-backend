namespace WebApp.Models;

public class LoginModel
{
    public required string UniId { get; set; }
    public required string PasswordHash { get; set; }
}