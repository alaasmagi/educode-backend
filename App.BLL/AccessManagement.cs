namespace App.BLL;
using BCrypt.Net;

public class AccessManagement
{
    public bool AdminAccessGrant(string enteredUsername, string enteredPassword)
    {
        var storedUsernameEnc = Environment.GetEnvironmentVariable("ADMINUSER") ?? "";
        var storedPasswordEnc = Environment.GetEnvironmentVariable("ADMINKEY") ?? "";
        
        var storedUsername = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(storedUsernameEnc));
        var storedPassword = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(storedPasswordEnc));
        
        return BCrypt.Verify(enteredUsername, storedUsername) && BCrypt.Verify(enteredPassword, storedPassword);
    }
    
}