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

    public AttendanceManagementService(AppDbContext context, ILogger<AttendanceManagementService> logger)
    {
        _logger = logger;
        _context = context;
        _attendanceRepository = new AttendanceRepository(_context); 
    }

    private async Task<bool> DoesAttendanceExist(int id)
    {
        var result = await _context.CourseAttendances.AnyAsync(u => u.Id == id);

        if (!result)
        {
            _logger.LogError($"Attendance with ID {id} was not found");
            return false;
        }
        
        _logger.LogInformation($"Attendance with ID {id} was found");
        return true;
    }
    
    public async Task<CourseAttendanceEntity?> GetCurrentAttendanceAsync(int userId)
    {
        var currentAttendance = await _attendanceRepository.GetCurrentAttendance(userId);

        if (currentAttendance == null)
        {
            _logger.LogError($"Current attendance for user with ID {userId} was not found");
            return null;
        }
        
        return currentAttendance;
    }
    
    public async Task<CourseAttendanceEntity?> GetCourseAttendanceByIdAsync(int attendanceId)
    {
        var courseAttendance = await _context.CourseAttendances
            .FirstOrDefaultAsync(u => u.Id == attendanceId);

        if (courseAttendance == null)
        {
            _logger.LogError($"Attendance with ID {attendanceId} was not found");
            return null;
        }
        
        return courseAttendance;
    }
    
    public async Task<List<CourseAttendanceEntity>?> GetAttendancesByCourseAsync(int courseId)
    {
        var attendances = await _context.CourseAttendances
            .Where(c => c.CourseId == courseId).ToListAsync();

        if (attendances.Count <= 0)
        {
            _logger.LogError($"Attendances by course with ID {courseId} were not found");
            return null;
        }
        
        return attendances;
    }

    public async Task<bool> AddAttendanceCheckAsync(AttendanceCheckEntity attendanceCheck, string creator)
    {
        if (!await _attendanceRepository.AddAttendanceCheck(attendanceCheck, creator))
        {
            _logger.LogError($"Attendance check adding failed");
            return false;
        }
        
        return true;
    }

    public async Task<List<AttendanceCheckEntity>?> GetAttendanceChecksByAttendanceIdAsync(int attendanceId)
    {
        var attendanceChecks = await _context.AttendanceChecks
            .Where(c => c.CourseAttendanceId == attendanceId).ToListAsync();
        
        if (attendanceChecks.Count <= 0)
        {
            _logger.LogError($"Attendance checks for attendance with ID {attendanceId} were not found");
            return null;
        }
        
        return attendanceChecks;
    }

    public async Task<CourseAttendanceEntity?> GetMostRecentAttendanceByUserAsync(int userId)
    {
        var attendance = await _context.CourseAttendances
            .Where(ca => ca.Course!.CourseTeacherEntities!
                .Any(ct => ct.TeacherId == userId))
            .OrderByDescending(ca => ca.EndTime)
            .FirstOrDefaultAsync();

        if (attendance == null)
        {
            _logger.LogError($"Most recent attendance for user with ID {userId} was not found");
            return null;
        }
        
        return attendance;
    }

    public async Task<AttendanceCheckEntity?> GetAttendanceCheckByIdAsync(int id)
    {
        var result = await _context.AttendanceChecks.FirstOrDefaultAsync(ca => ca.Id == id);

        if (result == null)
        {
            _logger.LogError($"AttendanceCheck with ID {id} was not found");
            return null;
        }
        
        return result ;
    }
    
    public async Task<List<AttendanceTypeEntity>?> GetAttendanceTypesAsync()
    {
        var result = await _context.AttendanceTypes.ToListAsync();

        if (result.Count <= 0)
        {
            _logger.LogError($"Attendance types were not found");
            return null;
        }
        
        return result;
    }

    public async Task<AttendanceTypeEntity?> GetAttendanceTypeByIdAsync(int attendanceTypeId)
    {
        var result = await _context.AttendanceTypes
            .FirstOrDefaultAsync(ca => ca.Id == attendanceTypeId);

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
                Course = attendance.Course,
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

    public async Task<bool> EditAttendanceAsync(int attendanceId, CourseAttendanceEntity updatedAttendance)
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
    
    public async Task<bool> DeleteAttendance(int id)
    {
        var attendance = await GetCourseAttendanceByIdAsync(id);
        if (attendance == null)
        {
            _logger.LogError($"Deleting attendance with ID {id} failed");
            return false;
        }
        
        var status = await _attendanceRepository.DeleteAttendanceEntity(attendance);
        
        if (!status)
        {
            _logger.LogError($"Deleting attendance with ID {id} failed");
            return false;
        }
        
        return true;
    }
    
    public async Task<bool> DeleteAttendanceCheck(int id)
    {
        var attendanceCheck = await GetAttendanceCheckByIdAsync(id);
        if (attendanceCheck == null)
        {
            _logger.LogError($"Deleting attendance check with ID {id} failed");
            return false;
        }
        
        var status = await _attendanceRepository.DeleteAttendanceCheckEntity(attendanceCheck);

        if (!status)
        {
            _logger.LogError($"Deleting attendance check with ID {id} failed");
            return false;
        }
        return true;
    }
}