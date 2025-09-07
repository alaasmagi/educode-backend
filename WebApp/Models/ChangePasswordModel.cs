namespace WebApp.Models;

public class ChangePasswordModel
{
    public required string Email { get; set; }
    public required string NewPassword { get; set; }
}