using App.DAL.EF;
using App.Domain;
using Microsoft.EntityFrameworkCore;

namespace App.BLL;

public class UserManagement
{
    private readonly AppDbContext _context;
    private readonly UserRepository User;

    public UserManagement(AppDbContext context)
    {
        _context = context;
        User = new UserRepository(_context); 
    }
    
    public async Task<UserEntity?> AuthenticateUser(string uniId, string password)
    {
        var userAuthData = await User.GetUserAuthEntityByUniIdOrStudentCode(uniId);

        if (userAuthData == null)
        {
            return null;
        }

        return VerifyPasswordHash(password, userAuthData.PasswordHash) ? userAuthData.User! : null;
    }

    public async Task<bool> CreateAccount(UserEntity user, UserAuthEntity userAuthData)
    {
        if (await DoesUserExist(user.UniId))
        {
            return false;
        }
        
        await User.AddUserEntityToDb(user);
        userAuthData.UserId = user.Id;
        await User.AddUserAuthEntityToDb(userAuthData);

        return true;
    }
    
    private bool VerifyPasswordHash(string enteredPassword, string storedHash)
    {
        return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);
    }

    private async Task<bool> DoesUserExist(string uniId)
    {
        return await _context.Users.AnyAsync(u => u.UniId == uniId);
    }

    public async Task<UserEntity?> GetUserByUniId(string uniId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.UniId == uniId);
    }
    
    public async Task<UserTypeEntity?> GetUserType (string userType)
    {
        return await _context.UserTypes.FirstOrDefaultAsync(u => u.UserType == userType) ?? null;
    }
    
    public string GetPasswordHash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }
}