using App.DAL.EF;
using App.Domain;
using Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace App.BLL;

public class AttendanceManagementService : IAttendanceManagementService
{
    private readonly ILogger<AttendanceManagementService> _logger;
    private readonly AppDbContext _context;
    private readonly AttendanceRepository _attendanceRepository;
    private readonly CourseRepository _courseRepository;
    private readonly UserRepository _userRepository;

    public AttendanceManagementService(AppDbContext context, ILogger<AttendanceManagementService> logger)
    {
        _logger = logger;
        _context = context;
        _attendanceRepository = new AttendanceRepository(_context); 
        _courseRepository = new CourseRepository(_context); 
        _userRepository = new UserRepository(_context); 
    }
    public async Task<bool> DoesWorkplaceExist(string workplaceIdentifier)
    {
        var result = await _attendanceRepository.WorkplaceAvailabilityCheckById(workplaceIdentifier);

        if (!result)
        {
            _logger.LogError($"Workplace with id {workplaceIdentifier} was not found");
            return false;
        }

        return true;
    }
    
    public async Task<bool> DoesAttendanceExist(Guid attendanceId)
    {
        var result = await _attendanceRepository.AttendanceAvailabilityCheckById(attendanceId);

        if (!result)
        {
            _logger.LogError($"Attendance with ID {attendanceId} was not found");
            return false;
        }
        
        _logger.LogInformation($"Attendance with ID {attendanceId} was found");
        return true;
    }
    
    public async Task<bool> DoesAttendanceCheckExist(string studentCode, string fullName, string attendanceIdentifier)
    {
        var result = await _attendanceRepository.AttendanceCheckAvailabilityCheck(studentCode, attendanceIdentifier);

        if (!result)
        {
            _logger.LogError($"AttendanceCheck with student code {studentCode}, fullname {fullName} and attendance identifier {attendanceIdentifier} was not found");
            return false;
        }
        
        _logger.LogInformation($"AttendanceCheck with student code {studentCode} and attendance identifier {attendanceIdentifier} was found");
        return true;
    }
    
    public async Task<CourseAttendanceEntity?> GetCurrentAttendanceAsync(Guid userId)
    {
        var currentAttendance = await _attendanceRepository.GetCurrentAttendance(userId);

        if (currentAttendance == null)
        {
            _logger.LogError($"Current attendance for user with ID {userId} was not found");
            return null;
        }
        
        return currentAttendance;
    }
    
    public async Task<CourseAttendanceEntity?> GetCourseAttendanceByIdAsync(Guid attendanceId, string email)
    {
        var courseAttendance = await _attendanceRepository.GetAttendanceById(attendanceId);

        if (courseAttendance == null)
        {
            _logger.LogError($"Attendance with ID {attendanceId} was not found");
            return null;
        }
        
        var accessible = await IsAttendanceAccessibleByUser(courseAttendance, email);
        if (!accessible)
        {
            _logger.LogError($"AttendanceCheck with ID {attendanceId} cannot be fetched");
            return null;
        }
        return courseAttendance;
    }

    public async Task<int> GetStudentsCountByAttendanceIdAsync(string attendanceIdentifier)
    {
        var result = await _attendanceRepository.GetStudentCountByAttendanceId(attendanceIdentifier);
        if (result <= 0)
        {
            _logger.LogError($"Attendance with identifier {attendanceIdentifier} has no attendance checks");
            return 0;
        }
        return result;
    }
    
    public async Task<List<CourseAttendanceEntity>?> GetAttendancesByCourseAsync(Guid courseId)
    {
        var attendances = await _attendanceRepository.GetCourseAttendancesByCourseId(courseId);

        if (attendances.Count <= 0)
        {
            _logger.LogError($"Attendances by course with ID {courseId} were not found");
            return null;
        }

        return attendances;
    }

    public async Task<bool> AddAttendanceCheckAsync(AttendanceCheckEntity attendanceCheck, string creator, string? workplaceIdentifer)
    {
        var attendanceCheckExist = await DoesAttendanceCheckExist(attendanceCheck.StudentCode, attendanceCheck.FullName, attendanceCheck.AttendanceIdentifier);

        if (attendanceCheckExist)
        {
            _logger.LogError($"Attendance check adding failed");
            return false;
        }
        
        bool status;
        attendanceCheck.StudentCode = attendanceCheck.StudentCode.ToUpper();
        if (workplaceIdentifer != null)
        {
            var workplace = await _attendanceRepository.GetWorkplaceByIdentifier(workplaceIdentifer);
            status = await _attendanceRepository.AddAttendanceCheck(attendanceCheck, creator, workplace);
        }
        else
        {
            status = await _attendanceRepository.AddAttendanceCheck(attendanceCheck, creator, null);
        }
        
        if (!status)
        {
            _logger.LogError($"Attendance check adding failed");
            return false;
        }
        
        return true;
    }

    public async Task<List<AttendanceCheckEntity>?> GetAttendanceChecksByAttendanceIdAsync(string attendanceIdentifier)
    {
        var attendanceChecks = await _attendanceRepository.GetAttendanceChecksByAttendanceIdentifier(attendanceIdentifier);
        
        if (attendanceChecks.Count <= 0)
        {
            _logger.LogError($"Attendance checks for attendance with identifier {attendanceIdentifier} were not found");
            return null;
        }
        
        return attendanceChecks;
    }

    public async Task<CourseAttendanceEntity?> GetMostRecentAttendanceByUserAsync(Guid userId)
    {
        var attendance = await _attendanceRepository.GetMostRecentAttendanceByUser(userId);

        if (attendance == null)
        {
            _logger.LogError($"Most recent attendance for user with ID {userId} was not found");
            return null;
        }
        
        return attendance;
    }

    public async Task<AttendanceCheckEntity?> GetAttendanceCheckByIdAsync(Guid attendanceCheckId, string email)
    {
        var result = await _attendanceRepository.GetAttendanceCheckById(attendanceCheckId);
        if (result == null)
        {
            _logger.LogError($"AttendanceCheck with ID {attendanceCheckId} was not found");
            return null;
        }
        
        var accessible = await IsAttendanceCheckAccessibleByUser(result, email);
        if (!accessible)
        {
            _logger.LogError($"AttendanceCheck with ID {attendanceCheckId} cannot be fetched");
            return null;
        }
        
        return result;
    }
    
    public async Task<List<AttendanceTypeEntity>?> GetAttendanceTypesAsync()
    {
        var result = await _attendanceRepository.GetAttendanceTypes();

        if (result.Count <= 0)
        {
            _logger.LogError($"Attendance types were not found");
            return null;
        }
        
        return result;
    }

    public async Task<AttendanceTypeEntity?> GetAttendanceTypeByIdAsync(Guid attendanceTypeId)
    {
        var result = await _attendanceRepository.GetAttendanceTypeById(attendanceTypeId);

        if (result == null)
        {
            _logger.LogError($"Attendance type with ID {attendanceTypeId} was not found");
            return null;
        }
        
        return result;
    }
    
    public async Task<bool> AddAttendanceAsync(CourseAttendanceEntity attendance, List<DateOnly> attendanceDates, 
                                                                                TimeOnly startTime, TimeOnly endTime)
    {
        var failureCount = 0;
        foreach (var date in attendanceDates)
        {
            var newAttendance = new CourseAttendanceEntity()
            {
                CourseId = attendance.CourseId,
                AttendanceTypeId = attendance.AttendanceTypeId,
                AttendanceType = attendance.AttendanceType,
                StartTime = date.ToDateTime(startTime),
                EndTime = date.ToDateTime(endTime),
                CreatedBy = attendance.CreatedBy,
                UpdatedBy = attendance.UpdatedBy
            };
            
            if (!await _attendanceRepository.AddAttendance(newAttendance))
            {
                _logger.LogError($"Attendance with date {date} was not added");
                failureCount++;
            }
        }

        if (failureCount > 0)
        {
            _logger.LogError($"{failureCount} attendances were not added");
            return false;
        }
        return true;
    }

    public async Task<bool> EditAttendanceAsync(Guid attendanceId, CourseAttendanceEntity updatedAttendance)
    {
        if (!await DoesAttendanceExist(attendanceId))
        {
            _logger.LogError($"Updating attendance with ID {attendanceId} failed");
            return false;
        }
        
        var status = await _attendanceRepository.UpdateAttendance(attendanceId, updatedAttendance);

        if (!status)
        {
            _logger.LogError($"Updating attendance with ID {attendanceId} failed");
            return false;
        }

        return true;
    }

    public async Task<bool> DeleteAttendance(Guid attendanceId, string email)
    {
        var attendance = await GetCourseAttendanceByIdAsync(attendanceId, email);
        if (attendance == null)
        {
            _logger.LogError($"Deleting attendance with ID {attendanceId} failed");
            return false;
        }
        
        var status = await _attendanceRepository.DeleteAttendanceEntity(attendance);
        
        if (!status)
        {
            _logger.LogError($"Deleting attendance with ID {attendanceId} failed");
            return false;
        }
        
        return true;
    }
    
    public async Task<bool> DeleteAttendanceCheck(Guid attendanceCheckId, string email)
    {
        var attendanceCheck = await GetAttendanceCheckByIdAsync(attendanceCheckId, email);
        if (attendanceCheck == null)
        {
            _logger.LogError($"Deleting attendance check with ID {attendanceCheckId} failed");
            return false;
        }
        
        var status = await _attendanceRepository.DeleteAttendanceCheckEntity(attendanceCheck);

        if (!status)
        {
            _logger.LogError($"Deleting attendance check with ID {attendanceCheckId} failed");
            return false;
        }
        return true;
    }

    public async Task<bool> IsAttendanceAccessibleByUser(CourseAttendanceEntity attendance, string email)
    {
        var user =  await _userRepository.GetUserByEmailAsync(email);
        if (user == null)
        {
            _logger.LogError($"User email {email} was not found");
            return false;
        }
        
        var result = await _courseRepository.CourseAccessibilityCheck(attendance.CourseId, user.Id);

        if (result <= 0)
        {
            _logger.LogError($"Attendance with ID {attendance.Id} is not accessible by user with email {email}");
            return false;
        }
        
        return true;
    }
    
    public async Task<bool> IsAttendanceCheckAccessibleByUser(AttendanceCheckEntity attendanceCheck, string email)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);
        if (user == null)
        {
            _logger.LogError($"User email {email} was not found");
            return false;
        }
        
        var attendance = await _attendanceRepository.GetAttendanceByIdentifier(attendanceCheck.AttendanceIdentifier);
        if (attendance == null)
        {
            _logger.LogError($"Attendance with identifier {attendanceCheck.AttendanceIdentifier} was not found");
            return false;
        }
        
        var result = await _courseRepository.CourseAccessibilityCheck(attendance.CourseId, user.Id);
        
        if (result <= 0)
        {
            _logger.LogError($"Attendance check with ID {attendanceCheck.Id} is not accessible by user with email {email}");
            return false;
        }
        
        return true;
    }
}