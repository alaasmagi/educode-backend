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
    
    public async Task<UserAuthEntity?> GetUserAuthDataByUserId(int userId)
    {
        return await context.UserAuthData
            .Include(ua => ua.User).ThenInclude(ua => ua!.UserType) 
            .FirstOrDefaultAsync(ua => ua.UserId == userId);
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

    public async Task<bool> UserAvailabilityCheckByUniId(string uniId)
    {
       return await context.Users.AnyAsync(u => u.UniId == uniId);
    }

    public async Task<UserEntity?> GetUserByUniIdAsync(string uniId)
    {
        return await context.Users.Include(x => x.UserType)
            .FirstOrDefaultAsync(x => x.UniId == uniId);
    }
    
    public async Task<UserEntity?> GetUserByIdAsync(int userId)
    {
        return await context.Users.FindAsync(userId);
    }
    
    public async Task<UserTypeEntity?> GetUserTypeEntity(string userType)
    {
        return await context.UserTypes.FirstOrDefaultAsync(u => u.UserType == userType);
    }

    public async Task<List<UserEntity>> GetAllUsersAsList()
    {
        return await context.Users.ToListAsync();
    }
    
    public async Task<bool> RemoveOldUsers(DateTime datePeriod)
    {
        var oldUsers = await context.Users
            .Where(u => u.UpdatedAt < datePeriod && u.Deleted == true)
            .ToListAsync();

        if (!oldUsers.Any())
        {
            return false;
        }

        context.Users.RemoveRange(oldUsers);
        await context.SaveChangesAsync();
        
        return true;
    }
    
    public async Task<bool> RemoveOldUserAuths(DateTime datePeriod)
    {
        var oldUserAuths = await context.UserAuthData
            .Where(u => u.UpdatedAt < datePeriod && u.Deleted == true)
            .ToListAsync();

        if (!oldUserAuths.Any())
        {
            return false;
        }

        context.UserAuthData.RemoveRange(oldUserAuths);
        await context.SaveChangesAsync();
        
        return true;
    }
}