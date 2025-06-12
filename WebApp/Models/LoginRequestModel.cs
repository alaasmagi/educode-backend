namespace WebApp.Models;

public class LoginRequestModel
{
    public required string UniId { get; set; }
    public required string Password { get; set; }
}