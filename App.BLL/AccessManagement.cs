namespace App.BLL;
using BCrypt.Net;

public class AccessManagement
{
    public bool AdminAccessGrant(string enteredUsername, string enteredPassword)
    {
        var storedUsername = Environment.GetEnvironmentVariable("ADMINUSER") ?? "";
        var storedPassword = Environment.GetEnvironmentVariable("ADMINKEY") ?? "";
        
        return BCrypt.Verify(enteredUsername, storedUsername) && BCrypt.Verify(enteredPassword, storedPassword);
    }
    
}