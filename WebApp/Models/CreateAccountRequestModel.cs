namespace WebApp.Models;

public class CreateAccountRequestModel
{
    public required string Fullname { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string UserRole { get; set; }
    public required string Creator { get; set; }
    public string? StudentCode { get; set; }
}