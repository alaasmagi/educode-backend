using App.DAL.EF;
using App.Domain;
using Contracts;
using Microsoft.EntityFrameworkCore;

namespace App.BLL;

public class UserManagementService : IUserManagementService
{
    private readonly AppDbContext _context;
    private readonly UserRepository _user;

    public UserManagementService(AppDbContext context)
    {
        _context = context;
        _user = new UserRepository(_context); 
    }
    
    public async Task<UserEntity?> AuthenticateUserAsync(int userId, string password)
    {
        var userAuthData = await _user.GetUserAuthEntityByUniIdOrStudentCode(userId);

        if (userAuthData == null)
        {
            return null;
        }

        return VerifyPasswordHash(password, userAuthData.PasswordHash) ? userAuthData.User! : null;
    }

    public async Task<bool> CreateAccountAsync(UserEntity user, UserAuthEntity userAuthData)
    {
        if (await DoesUserExistAsync(user.UniId))
        {
            return false;
        }
        
        await _user.AddUserEntityToDb(user);
        userAuthData.UserId = user.Id;
        await _user.AddUserAuthEntityToDb(userAuthData);

        return true;
    }

    public async Task<bool> ChangeUserPasswordAsync(UserEntity user, string newPasswordHash)
    { 
        return await _user.UpdateUserAuthEntity(user.Id, newPasswordHash);
    }
    
    private bool VerifyPasswordHash(string enteredPassword, string storedHash)
    {
        return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);
    }

    public async Task<bool> DoesUserExistAsync(string uniId)
    {
        return await _context.Users.AnyAsync(u => u.UniId == uniId);
    }
    
    public async Task<UserTypeEntity?> GetUserTypeAsync(string userType)
    {
        return await _context.UserTypes.FirstOrDefaultAsync(u => u.UserType == userType) ?? null;
    }

    public async Task<List<UserEntity>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }
    
    public async Task<UserEntity?> GetUserByUniIdAsync(string uniId)
    {
        return await _context.Users.Include(x => x.UserType)
        .FirstOrDefaultAsync(x => x.UniId == uniId);
    }
    
    public async Task<UserEntity?> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }
    
    public string GetPasswordHash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }
    
    public async Task<bool> DeleteUserAsync(UserEntity user)
    {
        await _user.DeleteUserEntity(user);
        return true;
    }
}