namespace WebApp.Models;

public class RefreshTokenRequestModel
{
    public required string JwtToken { get; set; }
    public required string RefreshToken { get; set; }
}