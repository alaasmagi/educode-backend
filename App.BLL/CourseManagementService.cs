using System.Text.Json;
using App.DAL.EF;
using App.Domain;
using App.DTO;
using Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;


namespace App.BLL;

public class CourseManagementService : ICourseManagementService
{
    private readonly AppDbContext _context;
    public readonly CourseRepository _courseRepository;
    public readonly AttendanceRepository _attendanceRepository;
    private readonly RedisRepository _redisRepository;
    private readonly UserRepository _userRepository;
    private readonly ILogger<CourseManagementService> _logger;

    public CourseManagementService(AppDbContext context, ILogger<CourseManagementService> logger, IConnectionMultiplexer connectionMultiplexer, ILogger<RedisRepository> redisLogger)
    {
        _logger = logger;
        _context = context;
        _courseRepository = new CourseRepository(_context);
        _attendanceRepository = new AttendanceRepository(_context);
        _redisRepository = new RedisRepository(connectionMultiplexer, redisLogger); 
        _userRepository = new UserRepository(_context);
    }

    public async Task<CourseEntity?> GetCourseByAttendanceIdAsync(Guid attendanceId)
    {
        var cache = await _redisRepository.GetDataAsync(Constants.CoursePrefix + 
                                                        Constants.AttendancePrefix + attendanceId);

        if (cache != null)
        {
            var cachedCourse = JsonSerializer.Deserialize<CourseEntity>(cache);
            return cachedCourse;
        }
        
        var courseAttendance = await _attendanceRepository.GetAttendanceById(attendanceId);

        if (courseAttendance == null)
        {
            _logger.LogError($"Course attendance with id {attendanceId} not found");
            return null;
        }
        
        var course = await _courseRepository.GetCourseById(courseAttendance.CourseId);

        if (course == null)
        {
            _logger.LogError($"Course with attendance with ID {attendanceId} was not found");
            return null;
        }
        
        var serializedCourse = JsonSerializer.Serialize(course);
        await _redisRepository.SetDataAsync(Constants.CoursePrefix + Constants.AttendancePrefix + attendanceId, 
                                                                                    serializedCourse, null);
        return course;
    }

    public async Task<CourseEntity?> GetCourseByIdAsync(Guid courseId, string email)
    {
        var cache = await _redisRepository.GetDataAsync(Constants.CoursePrefix + courseId);
        CourseEntity? course;
        
        if (cache != null)
        {
            course = JsonSerializer.Deserialize<CourseEntity>(cache);
        }
        else
        {
            course = await _courseRepository.GetCourseById(courseId);
        }
        
        if (course == null)
        {
            _logger.LogError($"Course with ID {courseId} was not found");
            return null;
        }
        
        var serializedCourse = JsonSerializer.Serialize(course);
        await _redisRepository.SetDataAsync(Constants.CoursePrefix + courseId, 
            serializedCourse, null);
        
        var accessible = await IsCourseAccessibleToUser(course, email);
        if (!accessible)
        {
            _logger.LogError($"Course with ID {course.Id} cannot be fetched");
            return null;
        }
        
        return course;
    }
    
    public async Task<CourseEntity?> GetCourseByCodeAsync(string courseCode, string email)
    {
        var cache = await _redisRepository.GetDataAsync(Constants.CoursePrefix + courseCode);
        CourseEntity? course;
        
        if (cache != null)
        {
            course = JsonSerializer.Deserialize<CourseEntity>(cache);
        }
        else
        {
            course = await _courseRepository.GetCourseByCode(courseCode);
        }
        
        if (course == null)
        {
            _logger.LogError($"Course with code {courseCode} was not found");
            return null;
        }
        
        var serializedCourse = JsonSerializer.Serialize(course);
        await _redisRepository.SetDataAsync(Constants.CoursePrefix + courseCode, 
            serializedCourse, null);
        
        var accessible = await IsCourseAccessibleToUser(course, email);
        if (!accessible)
        {
            _logger.LogError($"Course with ID {course.Id} cannot be fetched");
            return null;
        }
        
        return course;
    }
    
    public async Task<bool> AddCourse(UserEntity user, CourseEntity course, string creator)
    {
        var courseExists = await DoesCourseExistByCodeAsync(course.CourseCode);

        if (courseExists)
        {
            _logger.LogError($"Course with code {course.CourseCode} already exists");
            return false;
        }
        
        var courseTeacher = new CourseTeacherEntity
        {
            TeacherId = user.Id,
            CreatedBy = creator,
            UpdatedBy = creator
        };
        
        var status = await _courseRepository.AddCourseEntity(courseTeacher, course);

        if (!status)
        {
            _logger.LogError("Failed to add course");
            return false;
        }
        
        return true;
    }
    public async Task<bool> EditCourse(Guid courseId, CourseEntity newCourse)
    {
        var courseExistence = await DoesCourseExistByIdAsync(courseId);
        
        if (!courseExistence)
        {
            _logger.LogError($"Failed to update course with id {courseId}");
            return false;
        }
        
        await _redisRepository.DeleteDataAsync(Constants.CoursePrefix + courseId);
        
        var status = await _courseRepository.UpdateCourseEntity(courseId, newCourse);
        if (!status)
        {
            _logger.LogError($"Failed to update course with id {courseId}");
            return false;
        }
        
        return true;
    }
    public async Task<bool> DeleteCourse(Guid courseId, string email)
    {
        var course = await GetCourseByIdAsync(courseId, email);
        
        if (course == null)
        {
            _logger.LogError($"Failed to delete course with id {courseId}");
            return false;
        }
        
        await _redisRepository.DeleteDataAsync(Constants.CoursePrefix + courseId);

        var status = await _courseRepository.DeleteCourseEntity(course);
        
        if (!status)
        {
            _logger.LogError($"Failed to delete course with id {courseId}");
            return false;
        }
        
        return true;
    }
    
    public async Task<List<CourseStatusEntity>?> GetAllCourseStatuses()
    {
        var result = await _courseRepository.GetAllCourseStatuses();

        if (result != null && result.Count <= 0)
        {
            _logger.LogError($"Failed to get course statuses");
            return null;
        }
        
        return result;
    }
    
    public async Task<List<CourseEntity>?> GetCoursesByUserAsync(Guid userId, int pageNr, int pageSize)
    {
        var result = await _courseRepository.GetCoursesByUser(userId);

        if (result == null)
        {
            _logger.LogError($"Failed to get courses by user with ID {userId}");
            return null;
        }

        return result;
    }
    
    public async Task<List<AttendanceStudentCountDto>?> GetAttendancesUserCountsByCourseAsync(Guid courseId)
    {
        var result = await _courseRepository.GetAllUserCountsByCourseId(courseId);

        if (result == null)
        {
            _logger.LogError($"Failed to get attendances user counts by course with ID {courseId}");
            return null;
        }
        
        return result;
    }
    
    public async Task<bool> IsCourseAccessibleToUser(CourseEntity courseEntity, string email)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);
        
        if (user == null)
        {
            _logger.LogError($"User with email {email} was not found");
            return false;
        }

        var result = await _courseRepository.CourseAccessibilityCheck(courseEntity.Id, user.Id);
        if (result <= 0)
        {
            _logger.LogError($"Course with with ID {courseEntity.Id} is not accessible by user with email {email}");
            return false;
        }
        
        return true;
    }
    
    public async Task<bool> DoesCourseExistByCodeAsync(string courseCode)
    {
        var status = await _courseRepository.CourseAvailabilityCheckByCourseCode(courseCode);

        if (!status)
        {
            _logger.LogError($"Course with code {courseCode} was not found");
            return false;
        }
        
        _logger.LogInformation($"Course with code {courseCode} was found");
        return true;        
    }
    
    public async Task<bool> DoesCourseExistByIdAsync(Guid id)
    {
        var status = await _courseRepository.CourseAvailabilityCheckById(id);

        if (!status)
        {
            _logger.LogError($"Course with code {id} was not found");
            return false;
        }
        
        _logger.LogInformation($"Course with code {id} was found");
        return true;        
    }
}
   
    