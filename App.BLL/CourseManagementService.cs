using App.DAL.EF;
using App.Domain;
using Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace App.BLL;

public class CourseManagementService : ICourseManagementService
{
    private readonly AppDbContext _context;
    private readonly CourseRepository _courseRepository;
    private readonly ILogger<CourseManagementService> _logger;

    public CourseManagementService(AppDbContext context, ILogger<CourseManagementService> logger)
    {
        _logger = logger;
        _context = context;
        _courseRepository = new CourseRepository(_context);
    }

    public async Task<CourseEntity?> GetCourseByAttendanceIdAsync(int attendanceId)
    {
        var courseAttendance = await _context.CourseAttendances
            .FirstOrDefaultAsync(u => u.Id == attendanceId);

        if (courseAttendance == null)
        {
            _logger.LogError($"Course attendance with id {attendanceId} not found");
            return null;
        }
        
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseAttendance.CourseId);

        if (course == null)
        {
            _logger.LogError($"Course with attendance with ID {attendanceId} was not found");
            return null;
        }
        
        return course;
    }

    public async Task<CourseEntity?> GetCourseByIdAsync(int courseId, string uniId)
    {
        var result = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);

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
        var result = await _context.Courses.FirstOrDefaultAsync(c => c.CourseName == courseName);
        
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
        var result = await _context.Courses.FirstOrDefaultAsync(c => c.CourseCode == courseCode);
        
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
        var courseExists = await _context.Courses.AnyAsync(c => c.CourseCode == course.CourseCode);

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
        if (!await DoesCourseExistAsync(courseId))
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
    
    public async Task<bool> DoesCourseExistAsync(int id)
    {
        var status = await _context.Courses.AnyAsync(u => u.Id == id);

        if (!status)
        {
            _logger.LogError($"Course with ID {id} was not found");
            return false;
        }
        
        _logger.LogInformation($"Course with ID {id} was found");
        return true;        
    }

    public async Task<bool> IsCourseAccessibleToUser(CourseEntity courseEntity, string uniId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UniId == uniId);
        var result = await _context.CourseTeachers
            .CountAsync(ct => ct.TeacherId == user!.Id && ct.CourseId == courseEntity.Id);
        
        if (result <= 0)
        {
            _logger.LogError($"Course with with ID {courseEntity.Id} is not accessible by user with UNI-ID {uniId}");
            return false;
        }
        
        return true;
    }
}
   
    