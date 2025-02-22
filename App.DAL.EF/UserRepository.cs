using App.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace App.DAL.EF;

public class UserRepository (AppDbContext context)
{
    // Base features
    public void AddUserEntityToDb(UserEntity newUser)
    {
        context.Users.Add(newUser);
        context.SaveChanges();
    }

    public void AddUserAuthEntityToDb(UserAuthEntity newUserAuth)
    {
        context.UserAuthData.Add(newUserAuth);
        context.SaveChanges();
    }
    
    
    // Business logic DB extensions
    public UserAuthEntity? GetUserAuthEntityByUsername(string username)
    {
        return context.UserAuthData
            .Include(ua => ua.User) 
            .FirstOrDefault(ua => ua.User!.UniId == username || ua.User.StudentCode == username);
    }
}