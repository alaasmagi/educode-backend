using System.Security.Cryptography;
using System.Text;
using Contracts;

namespace App.BLL;
using BCrypt.Net;

public class AdminAccessService : IAdminAccessService
{
    public bool AdminAccessGrant(string enteredUsername, string enteredPassword)
    {
        var storedUsernameEnc = Environment.GetEnvironmentVariable("ADMINUSER") ?? "";
        var storedPasswordEnc = Environment.GetEnvironmentVariable("ADMINKEY") ?? "";

        var storedUsername = Encoding.UTF8.GetString(Convert.FromBase64String(storedUsernameEnc));
        var storedPassword = Encoding.UTF8.GetString(Convert.FromBase64String(storedPasswordEnc));

        return BCrypt.Verify(enteredUsername, storedUsername) && BCrypt.Verify(enteredPassword, storedPassword);
    }

    public string GenerateAdminAccessToken()
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }

    public async Task<string> GetHashedAdminAccessTokenAsync(string input)
    {
        var salt = Environment.GetEnvironmentVariable("ADMINTOKENSALT");
        using (var sha256 = SHA256.Create())
        {
            var combined = input + salt;
            byte[] inputBytes = Encoding.UTF8.GetBytes(combined);

            using (var memoryStream = new MemoryStream(inputBytes))
            {
                byte[] hashBytes = await sha256.ComputeHashAsync(memoryStream);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }

    public async Task<bool> CompareHashedTokensAsync(string inputToken, string hashedInputToken)
    {
        var hashedToken = await GetHashedAdminAccessTokenAsync(inputToken);
        return hashedToken == hashedInputToken;
    }
}