namespace WebApp.Models;

public class AdminLoginModel
{
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string?  Message { get; set; } = default!;
}