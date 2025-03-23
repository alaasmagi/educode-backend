using System.Security.Cryptography;
using System.Text;
using Contracts;
using Microsoft.Extensions.Logging;

namespace App.BLL;
using BCrypt.Net;

public class AdminAccessService : IAdminAccessService
{
    private readonly ILogger<AdminAccessService> _logger;
    public AdminAccessService(ILogger<AdminAccessService> logger)
    {
        _logger = logger;
    }
    
    public bool AdminAccessGrant(string enteredUsername, string enteredPassword)
    {
        var storedUsernameEnc = Environment.GetEnvironmentVariable("ADMINUSER") ?? "";
        var storedPasswordEnc = Environment.GetEnvironmentVariable("ADMINKEY") ?? "";

        if (storedUsernameEnc == "" && storedPasswordEnc == "")
        {
            _logger.LogError("Reading data from env failed (ADMINUSER or ADMINKEY)");
            return false;
        }

        var storedUsername = Encoding.UTF8.GetString(Convert.FromBase64String(storedUsernameEnc));
        var storedPassword = Encoding.UTF8.GetString(Convert.FromBase64String(storedPasswordEnc));
        
        if (BCrypt.Verify(enteredUsername, storedUsername) && 
            BCrypt.Verify(enteredPassword, storedPassword))
        {
            return true;
        }
        
        _logger.LogError("Admin access grant failed");
        return false;
    }

    public string GenerateAdminAccessToken()
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));
        
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }

    public async Task<string> GetHashedAdminAccessTokenAsync(string input)
    {
        var salt = Environment.GetEnvironmentVariable("ADMINTOKENSALT");

        if (salt == null)
        {
            _logger.LogError("Reading data from env failed (ADMINTOKENSALT)");
            return string.Empty;
        }
        
        using var sha256 = SHA256.Create();
        var combined = input + salt;
        var inputBytes = Encoding.UTF8.GetBytes(combined);

        using var memoryStream = new MemoryStream(inputBytes);
        var hashBytes = await sha256.ComputeHashAsync(memoryStream);

        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }

    public async Task<bool> CompareHashedTokensAsync(string inputToken, string hashedInputToken)
    {
        var hashedToken = await GetHashedAdminAccessTokenAsync(inputToken);
        if (hashedToken == hashedInputToken)
        {
            return true;    
        }
        
        _logger.LogError("Hashed tokens do not match");
        return false;
    }
}