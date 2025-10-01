using App.DAL.EF;
using App.Domain;
using Contracts;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace App.BLL;

public class UserManagementService : IUserManagementService
{
    private readonly UserRepository _userRepository;
    private readonly CourseRepository _courseRepository;
    private readonly RedisRepository _redisRepository;
    private readonly ILogger<UserManagementService> _logger;

    public UserManagementService(AppDbContext context, ILogger<UserManagementService> logger, IConnectionMultiplexer connectionMultiplexer, ILogger<RedisRepository> redisLogger)
    {
        _userRepository = new UserRepository(context); 
        _courseRepository = new CourseRepository(context); 
        _redisRepository = new RedisRepository(connectionMultiplexer, redisLogger); 
        _logger = logger;
    }
    
    public async Task<UserEntity?> AuthenticateUserAsync(Guid userId, string password)
    {
        var userAuthData = await _userRepository.GetUserAuthDataByUserId(userId);

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
        if (await DoesUserExistAsync(user.Email))
        {
            _logger.LogError($"Failed to create account for user with email {user.Email}");
            return false;
        }

        if (user.StudentCode != null)
        {
            user.StudentCode = user.StudentCode.ToUpper();    
        }
        
        if (!await _userRepository.AddUserEntityToDb(user))
        {
            _logger.LogError($"Failed to create account for user with email {user.Email}");
            return false;
        }
        
        userAuthData.UserId = user.Id;
        if (!await _userRepository.AddUserAuthEntityToDb(userAuthData))
        {
            _logger.LogError($"Failed to create account for user with email {user.Email}");
            return false;
        }

        return true;
    }

    public async Task<bool> ChangeUserPasswordAsync(UserEntity user, string newPasswordHash)
    { 
        var status = await _userRepository.UpdateUserAuthEntity(user.Id, newPasswordHash);
        
        if (!status)
        {
            _logger.LogError($"Failed to change user password for user with ID {user.Id}");
            return false;
        }

        return true;
    }
    
    private static bool VerifyPasswordHash(string enteredPassword, string storedHash)
    {
        return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);
    }

    public async Task<bool> DoesUserExistAsync(string email)
    {
        var status = await _userRepository.UserAvailabilityCheckByEmail(email);
        
        if (!status)
        {
            _logger.LogError($"User with email {email} was not found");
            return false;
        }
        
        _logger.LogInformation($"User with email {email} was found");
        return true;
    }
    
    public async Task<UserTypeEntity?> GetUserTypeAsync(string userType)
    {
        var result = await _userRepository.GetUserTypeEntity(userType);

        if (result == null)
        {
            _logger.LogError($"Failed to get user type {userType}");
            return null;
        }
        
        return  result;
    }

    public async Task<List<UserEntity>?> GetAllUsersAsync(int pageNr, int pageSize)
    {
        var result = await _userRepository.GetAllUsersAsync(pageNr, pageSize);

        if (result.Count <= 0)
        {
            _logger.LogError("Failed to get all users");
            return null;
        }

        return result;
    }
    
    public async Task<UserEntity?> GetUserByEmailAsync(string email)
    {
        var result = await _userRepository.GetUserByEmailAsync(email);
        
        if (result == null)
        {
            _logger.LogError($"User with email {email} not found");
            return null;
        }

        return result;
    }
    
    public async Task<UserEntity?> GetUserByIdAsync(Guid id)
    {
        var result = await _userRepository.GetUserByIdAsync(id);

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
        await _courseRepository.DeleteCoursesByUserAsync(user.Id);
        bool status = await _userRepository.DeleteUserEntity(user);
        if (!status)
        {
            _logger.LogError($"Failed to delete user with ID {user.Id}");
            return false;
        }

        return true;
    }
}