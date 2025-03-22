using App.Domain;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF;

public class UserRepository (AppDbContext context)
{
    // Base features
    public async Task AddUserEntityToDb(UserEntity newUser)
    {
        newUser.CreatedAt = DateTime.Now.ToUniversalTime();
        newUser.UpdatedAt = DateTime.Now.ToUniversalTime();
        
        await context.Users.AddAsync(newUser);
        await context.SaveChangesAsync();
    }

    public async Task AddUserAuthEntityToDb(UserAuthEntity newUserAuth)
    {
        newUserAuth.CreatedAt = DateTime.Now.ToUniversalTime();
        newUserAuth.UpdatedAt = DateTime.Now.ToUniversalTime();
        
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

    public async Task<bool> UpdateUserAuthEntity(int userId, string newPasswordHash)
    {
        var userAuth = await context.UserAuthData.FirstOrDefaultAsync(u => u.UserId == userId);

        if (userAuth == null)
        {
            return false;
        }
        
        userAuth.UpdatedAt = DateTime.Now.ToUniversalTime();
        userAuth.PasswordHash = newPasswordHash;
        await context.SaveChangesAsync();
        
        return true;
    }

    public async Task DeleteUserEntity(UserEntity user)
    {
        context.Users.Remove(user);
        await context.SaveChangesAsync();
    }
    
}