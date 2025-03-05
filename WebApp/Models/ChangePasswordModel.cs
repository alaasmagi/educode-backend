namespace WebApp.Models;

public class ChangePasswordModel
{
    public required string UniId { get; set; }
    public required string NewPassword { get; set; }
}