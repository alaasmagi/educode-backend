using App.DAL.EF;
using App.Domain;
using Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace App.BLL;

public class UserManagementService : IUserManagementService
{
    private readonly AppDbContext _context;
    private readonly UserRepository _userRepository;
    private readonly CourseRepository _courseRepository;
    private readonly ILogger<UserManagementService> _logger;

    public UserManagementService(AppDbContext context, ILogger<UserManagementService> logger)
    {
        _context = context;
        _userRepository = new UserRepository(_context); 
        _courseRepository = new CourseRepository(_context); 
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
        if (await DoesUserExistAsync(user.UniId))
        {
            _logger.LogError($"Failed to create account for user with ID {user.UniId}");
            return false;
        }

        if (user.StudentCode != null)
        {
            user.StudentCode = user.StudentCode.ToUpper();    
        }
        
        if (!await _userRepository.AddUserEntityToDb(user))
        {
            _logger.LogError($"Failed to create account for user with ID {user.UniId}");
            return false;
        }
        
        userAuthData.UserId = user.Id;
        if (!await _userRepository.AddUserAuthEntityToDb(userAuthData))
        {
            _logger.LogError($"Failed to create account for user with ID {user.UniId}");
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

    public async Task<bool> DoesUserExistAsync(string uniId)
    {
        var status = await _userRepository.UserAvailabilityCheckByUniId(uniId);
        
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
        var result = await _userRepository.GetUserTypeEntity(userType);

        if (result == null)
        {
            _logger.LogError($"Failed to get user type {userType}");
            return null;
        }
        
        return  result;
    }

    public async Task<List<UserEntity>?> GetAllUsersAsync()
    {
        var result = await _userRepository.GetAllUsersAsList();

        if (result.Count <= 0)
        {
            _logger.LogError("Failed to get all users");
            return null;
        }

        return result;
    }
    
    public async Task<UserEntity?> GetUserByUniIdAsync(string uniId)
    {
        var result = await _userRepository.GetUserByUniIdAsync(uniId);
        
        if (result == null)
        {
            _logger.LogError($"User with UNI-ID {uniId} not found");
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
        var courses = await _courseRepository.GetCoursesByUser(user.Id);
        if (courses == null || !courses.Any())
            return await FinalizeUserDeletion(user);

        foreach (var course in courses)
        {
            bool isOnlyTeacher = await _courseRepository.CourseOnlyTeacherCheck(user.Id, course.Id);
            if (!isOnlyTeacher)
                continue;

            bool courseDeleted = await _courseRepository.DeleteCourseEntity(course);
            if (!courseDeleted)
                _logger.LogWarning($"Failed to delete course {course.Id} while deleting user {user.Id}");
        }

        return await FinalizeUserDeletion(user);
    }

    private async Task<bool> FinalizeUserDeletion(UserEntity user)
    {
        bool status = await _userRepository.DeleteUserEntity(user);
        if (!status)
        {
            _logger.LogError($"Failed to delete user with ID {user.Id}");
            return false;
        }

        return true;
    }
}