using App.DAL.EF;
using App.Domain;
using Microsoft.EntityFrameworkCore;

namespace App.BLL;

public class UserManagement(AppDbContext context)
{
    public bool AddUser(UserEntity newUser)
    {
        context.Users.Add(newUser);
        context.SaveChanges();
        return true;
    }
    
    public UserEntity AuthenticateUser(string username, string password)
    {
        var userAuthData = context.UserAuthData
            .Include(ua => ua.User) 
            .FirstOrDefault(ua => ua.User!.UniId == username || ua.User.StudentCode == username);

        if (userAuthData == null)
        {
            return null;
        }

        if (VerifyPasswordHash(password, userAuthData.PasswordHash))
        {
            return userAuthData.User!; 
        }

        return null;
    }

    private bool VerifyPasswordHash(string enteredPassword, string storedHash)
    {
        return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);
    }
}