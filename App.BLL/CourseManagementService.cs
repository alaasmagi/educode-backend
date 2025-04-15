using App.DAL.EF;
using App.Domain;
using Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace App.BLL;

public class CourseManagementService : ICourseManagementService
{
    private readonly AppDbContext _context;
    public readonly CourseRepository _courseRepository;
    public readonly AttendanceRepository _attendanceRepository;
    private readonly UserRepository _userRepository;
    private readonly ILogger<CourseManagementService> _logger;

    public CourseManagementService(AppDbContext context, ILogger<CourseManagementService> logger)
    {
        _logger = logger;
        _context = context;
        _courseRepository = new CourseRepository(_context);
        _attendanceRepository = new AttendanceRepository(_context);
        _userRepository = new UserRepository(_context);
    }

    public async Task<CourseEntity?> GetCourseByAttendanceIdAsync(int attendanceId)
    {
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
        
        return course;
    }

    public async Task<CourseEntity?> GetCourseByIdAsync(int courseId, string uniId)
    {
        var result = await _courseRepository.GetCourseById(courseId);

        if (result == null)
        {
            _logger.LogError($"Course with ID {courseId} was not found");
            return null;
        }
        
        var accessible = await IsCourseAccessibleToUser(result, uniId);
        if (!accessible)
        {
            _logger.LogError($"Course with ID {result.Id} cannot be fetched");
            return null;
        }
        
        return result;
    }
    
    public async Task<CourseEntity?> GetCourseByNameAsync(string courseName, string uniId)
    {
        var result = await _courseRepository.GetCourseByName(courseName);
        
        if (result == null)
        {
             _logger.LogError($"Course with name {courseName} was not found");
            return null;
        }
        
        var accessible = await IsCourseAccessibleToUser(result, uniId);
        if (!accessible)
        {
            _logger.LogError($"Course with ID {result.Id} cannot be fetched");
            return null;
        }
        
        return result;
    }
    
    public async Task<CourseEntity?> GetCourseByCodeAsync(string courseCode, string uniId)
    {
        var result = await _courseRepository.GetCourseByCode(courseCode);
        
        if (result == null)
        {
            _logger.LogError($"Course with code {courseCode} was not found");
            return null;
        }
        
        var accessible = await IsCourseAccessibleToUser(result, uniId);
        if (!accessible)
        {
            _logger.LogError($"Course with ID {result.Id} cannot be fetched");
            return null;
        }
        
        return result;
    }
    
    public async Task<bool> AddCourse(UserEntity user, CourseEntity course, string creator)
    {
        var courseExists = await DoesCourseExistAsync(course.CourseCode);

        if (courseExists)
        {
            _logger.LogError($"Course with code {course.CourseCode} already exists");
            return false;
        }
        
        var courseTeacher = new CourseTeacherEntity
        {
            Teacher = user,
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
    public async Task<bool> EditCourse(int courseId, CourseEntity newCourse)
    {
        var courseExistence = await DoesCourseExistByIdAsync(courseId);
        if (!courseExistence)
        {
            _logger.LogError($"Failed to update course with id {courseId}");
            return false;
        }
        
        var status = await _courseRepository.UpdateCourseEntity(courseId, newCourse);
        if (!status)
        {
            _logger.LogError($"Failed to update course with id {courseId}");
            return false;
        }
        
        return true;
    }
    public async Task<bool> DeleteCourse(int id, string uniId)
    {
        var course = await GetCourseByIdAsync(id, uniId);
        
        if (course == null)
        {
            _logger.LogError($"Failed to delete course with id {id}");
            return false;
        }
        
        var status = await _courseRepository.DeleteCourseEntity(course);
        
        if (!status)
        {
            _logger.LogError($"Failed to delete course with id {id}");
            return false;
        }
        
        return true;
    }
    
    public List<CourseStatusDto>? GetAllCourseStatuses()
    {
        var result = Enum.GetValues(typeof(ECourseValidStatus))
            .Cast<ECourseValidStatus>()
            .Select(status => new CourseStatusDto
            {
                Id = (int)status,
                Status = status.ToString().ToLower()
            })
            .ToList();

        if (result.Count <= 0)
        {
            _logger.LogError($"Failed to get course statuses");
            return null;
        }
        
        return result;
    }
    
    public async Task<List<CourseEntity>?> GetCoursesByUserAsync(int userId)
    {
        var result = await _courseRepository.GetCoursesByUser(userId);

        if (result == null)
        {
            _logger.LogError($"Failed to get courses by user with ID {userId}");
            return null;
        }

        return result;
    }
    
    public async Task<List<CourseUserCountDto>?> GetAttendancesUserCountsByCourseAsync(int courseId)
    {
        var result = await _courseRepository.GetAllUserCountsByCourseId(courseId);

        if (result == null)
        {
            _logger.LogError($"Failed to get attendances user counts by course with ID {courseId}");
            return null;
        }
        
        return result;
    }
    
    public async Task<bool> IsCourseAccessibleToUser(CourseEntity courseEntity, string uniId)
    {
        var user = await _userRepository.GetUserByUniIdAsync(uniId);
        
        if (user == null)
        {
            _logger.LogError($"User with uniId {uniId} was not found");
            return false;
        }

        var result = await _courseRepository.CourseAccessibilityCheck(courseEntity.Id, user.Id);
        if (result <= 0)
        {
            _logger.LogError($"Course with with ID {courseEntity.Id} is not accessible by user with UNI-ID {uniId}");
            return false;
        }
        
        return true;
    }
    
    public async Task<bool> DoesCourseExistAsync(string courseCode)
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
    
    public async Task<bool> DoesCourseExistByIdAsync(int id)
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
   
    