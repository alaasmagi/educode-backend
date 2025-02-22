using System.Security.Cryptography;
using System.Text;

namespace App.BLL;
using BCrypt.Net;

public class AdminAccessManagement
{
    public bool AdminAccessGrant(string enteredUsername, string enteredPassword)
    {
        var storedUsernameEnc = Environment.GetEnvironmentVariable("ADMINUSER") ?? "";
        var storedPasswordEnc = Environment.GetEnvironmentVariable("ADMINKEY") ?? "";
        
        var storedUsername = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(storedUsernameEnc));
        var storedPassword = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(storedPasswordEnc));
        
        return BCrypt.Verify(enteredUsername, storedUsername) && BCrypt.Verify(enteredPassword, storedPassword);
    }
    
    public string GenerateAdminAccessToken()
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }

    public string GetHashedAdminAccessToken(string input)
    {
        var salt = Environment.GetEnvironmentVariable("ADMINTOKENSALT");
        using (var sha256 = SHA256.Create())
        {
            var combined = input + salt;
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
    
    public bool CompareHashedTokens(string inputToken, string hashedInputToken)
    {
        var hashedToken = GetHashedAdminAccessToken(inputToken);
        return hashedToken == hashedInputToken;
    }
}