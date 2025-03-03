using App.Domain;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF;

public class UserRepository (AppDbContext context)
{
    // Base features
    public async Task AddUserEntityToDb(UserEntity newUser)
    {
        newUser.CreatedAt = DateTime.Now;
        newUser.UpdatedAt = DateTime.Now;
        
        await context.Users.AddAsync(newUser);
        await context.SaveChangesAsync();
    }

    public async Task AddUserAuthEntityToDb(UserAuthEntity newUserAuth)
    {
        newUserAuth.CreatedAt = DateTime.Now;
        newUserAuth.UpdatedAt = DateTime.Now;
        
        await context.UserAuthData.AddAsync(newUserAuth);
        await context.SaveChangesAsync();
    }
    
    
    // Business logic DB extensions
    public async Task<UserAuthEntity?> GetUserAuthEntityByUniIdOrStudentCode(string input)
    {
        return await context.UserAuthData
            .Include(ua => ua.User).ThenInclude(ua => ua!.UserType) 
            .FirstOrDefaultAsync(ua => ua.User!.UniId == input || ua.User.StudentCode == input) ?? null;
    }
}