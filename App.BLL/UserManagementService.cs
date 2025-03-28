﻿using App.DAL.EF;
using App.Domain;
using Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace App.BLL;

public class UserManagementService : IUserManagementService
{
    private readonly AppDbContext _context;
    private readonly UserRepository _user;
    private readonly ILogger<UserManagementService> _logger;

    public UserManagementService(AppDbContext context, ILogger<UserManagementService> logger)
    {
        _context = context;
        _user = new UserRepository(_context); 
        _logger = logger;
    }
    
    public async Task<UserEntity?> AuthenticateUserAsync(int userId, string password)
    {
        var userAuthData = await _user.GetUserAuthEntityByUniIdOrStudentCode(userId);

        if (userAuthData == null)
        {
            _logger.LogError($"Failed to fetch user auth data for user with ID {userId}");
            return null;
        }

        var result = VerifyPasswordHash(password, userAuthData.PasswordHash);
        
        if (!result)
        {
            _logger.LogError($"Failed to authenticate user with ID {userId}");
            return null;
        }
        
        return userAuthData.User;
    }

    public async Task<bool> CreateAccountAsync(UserEntity user, UserAuthEntity userAuthData)
    {
        if (await DoesUserExistAsync(user.UniId))
        {
            _logger.LogError($"Failed to create account for user with ID {user.UniId}");
            return false;
        }

        if (!await _user.AddUserEntityToDb(user))
        {
            _logger.LogError($"Failed to create account for user with ID {user.UniId}");
            return false;
        }
        
        userAuthData.UserId = user.Id;
        if (!await _user.AddUserAuthEntityToDb(userAuthData))
        {
            _logger.LogError($"Failed to create account for user with ID {user.UniId}");
            return false;
        }

        return true;
    }

    public async Task<bool> ChangeUserPasswordAsync(UserEntity user, string newPasswordHash)
    { 
        var status = await _user.UpdateUserAuthEntity(user.Id, newPasswordHash);
        
        if (!status)
        {
            _logger.LogError($"Failed to change user password for user with ID {user.Id}");
            return false;
        }

        return true;
    }
    
    private bool VerifyPasswordHash(string enteredPassword, string storedHash)
    {
        return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);
    }

    public async Task<bool> DoesUserExistAsync(string uniId)
    {
        var status = await _context.Users.AnyAsync(u => u.UniId == uniId);
        
        if (!status)
        {
            _logger.LogError($"User with UNI-ID {uniId} was not found");
            return false;
        }
        
        _logger.LogInformation($"User with UNI-ID {uniId} was found");
        return true;
    }
    
    public async Task<UserTypeEntity?> GetUserTypeAsync(string userType)
    {
        var result = await _context.UserTypes.FirstOrDefaultAsync(u => u.UserType == userType);

        if (result == null)
        {
            _logger.LogError($"Failed to get user type {userType}");
            return null;
        }
        
        return  result;
    }

    public async Task<List<UserEntity>?> GetAllUsersAsync()
    {
        var result = await _context.Users.ToListAsync();

        if (result.Count <= 0)
        {
            _logger.LogError("Failed to get all users");
            return null;
        }

        return result;
    }
    
    public async Task<UserEntity?> GetUserByUniIdAsync(string uniId)
    {
        var result = await _context.Users.Include(x => x.UserType)
            .FirstOrDefaultAsync(x => x.UniId == uniId);
        
        if (result == null)
        {
            _logger.LogError($"User with UNI-ID {uniId} not found");
            return null;
        }

        return result;
    }
    
    public async Task<UserEntity?> GetUserByIdAsync(int id)
    {
        var result = await _context.Users.FindAsync(id);

        if (result == null)
        {
            _logger.LogError($"User with ID {id} not found");
            return null;
        }
        
        return result;
    }
    
    public string GetPasswordHash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }
    
    public async Task<bool> DeleteUserAsync(UserEntity user)
    {
        var status = await _user.DeleteUserEntity(user);

        if (!status)
        {
            _logger.LogError($"Failed to delete user with ID {user.Id}");
            return false;
        }
        
        return true;
    }
}