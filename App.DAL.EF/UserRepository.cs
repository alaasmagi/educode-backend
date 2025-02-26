using App.Domain;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF;

public class UserRepository (AppDbContext context)
{
    // Base features
    public void AddUserEntityToDb(UserEntity newUser)
    {
        newUser.CreatedAt = DateTime.Now;
        newUser.UpdatedAt = DateTime.Now;
        
        context.Users.Add(newUser);
        context.SaveChanges();
    }

    public void AddUserAuthEntityToDb(UserAuthEntity newUserAuth)
    {
        newUserAuth.CreatedAt = DateTime.Now;
        newUserAuth.UpdatedAt = DateTime.Now;
        
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