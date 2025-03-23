using App.Domain;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF;

public class UserRepository (AppDbContext context)
{
    public async Task<bool> AddUserEntityToDb(UserEntity newUser)
    {
        newUser.CreatedAt = DateTime.Now.ToUniversalTime();
        newUser.UpdatedAt = DateTime.Now.ToUniversalTime();
        
        await context.Users.AddAsync(newUser);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> AddUserAuthEntityToDb(UserAuthEntity newUserAuth)
    {
        newUserAuth.CreatedAt = DateTime.Now.ToUniversalTime();
        newUserAuth.UpdatedAt = DateTime.Now.ToUniversalTime();
        
        await context.UserAuthData.AddAsync(newUserAuth);
        return await context.SaveChangesAsync() > 0;
    }
    
    public async Task<UserAuthEntity?> GetUserAuthEntityByUniIdOrStudentCode(int input)
    {
        return await context.UserAuthData
            .Include(ua => ua.User).ThenInclude(ua => ua!.UserType) 
            .FirstOrDefaultAsync(ua => ua.UserId == input) ?? null;
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
        
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteUserEntity(UserEntity user)
    {
        context.Users.Remove(user);
        return await context.SaveChangesAsync() > 0;
    }
    
}