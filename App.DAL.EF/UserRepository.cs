using App.Domain;
using Microsoft.EntityFrameworkCore;

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
    public UserAuthEntity? GetUserAuthEntityByUniIdOrStudentCode(string input)
    {
        return context.UserAuthData
            .Include(ua => ua.User).ThenInclude(ua => ua!.UserType) 
            .FirstOrDefault(ua => ua.User!.UniId == input || ua.User.StudentCode == input) ?? null;
    }
}