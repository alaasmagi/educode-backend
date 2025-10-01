namespace WebApp.Models;

public class LoginRequestModel
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Client { get; set; }
}