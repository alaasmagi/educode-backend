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
    public async Task<bool> DoesWorkplaceExist(int id)
    {
        var result = await _context.Workplaces.AnyAsync(w => w.Id == id);

        if (!result)
        {
            _logger.LogError($"Workplace with id {id} was not found");
            return false;
        }

        return true;
    }
    
    public async Task<bool> DoesAttendanceExist(int id)
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
    
    public async Task<bool> DoesAttendanceCheckExist(string studentCode, int attendanceId)
    {
        var result = await _context.AttendanceChecks.AnyAsync(u => u.StudentCode == studentCode 
                                                                   && u.CourseAttendanceId == attendanceId);

        if (!result)
        {
            _logger.LogError($"AttendanceCheck with student code {studentCode} and attendance ID {attendanceId} was not found");
            return false;
        }
        
        _logger.LogInformation($"AttendanceCheck with student code {studentCode} and attendance ID {attendanceId} was found");
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
    
    public async Task<CourseAttendanceEntity?> GetCourseAttendanceByIdAsync(int attendanceId, string uniId)
    {
        var courseAttendance = await _context.CourseAttendances
            .Where(u => u.Id == attendanceId)
            .Include(u => u.Course).Include(u => u.AttendanceType)
            .FirstOrDefaultAsync();

        if (courseAttendance == null)
        {
            _logger.LogError($"Attendance with ID {attendanceId} was not found");
            return null;
        }
        
        var accessible = await IsAttendanceAccessibleByUser(courseAttendance, uniId);
        if (!accessible)
        {
            _logger.LogError($"AttendanceCheck with ID {attendanceId} cannot be fetched");
            return null;
        }
        return courseAttendance;
    }

    public async Task<int> GetStudentsCountByAttendanceIdAsync(int attendanceId)
    {
        var result = await _attendanceRepository.GetStudentCountByAttendanceId(attendanceId);
        if (result <= 0)
        {
            _logger.LogError($"Attendance with ID {attendanceId} has no attendance checks");
            return 0;
        }
        return result;
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

    public async Task<bool> AddAttendanceCheckAsync(AttendanceCheckEntity attendanceCheck, string creator, int? workplaceId)
    {
        
        var attendanceCheckExist = await DoesAttendanceCheckExist(attendanceCheck.StudentCode, attendanceCheck.CourseAttendanceId);

        if (attendanceCheckExist)
        {
            _logger.LogError($"Attendance check adding failed");
            return false;
        }
        
        bool status;
        if (workplaceId != null)
        {
            var workplace = await _context.Workplaces.FirstOrDefaultAsync(w => w.Id == workplaceId);
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
                .Any(ct => ct.TeacherId == userId) && ca.StartTime <= DateTime.Now) 
            .Include(ca => ca.Course)
            .Include(ca => ca.AttendanceType) 
            .OrderByDescending(ca => ca.EndTime) 
            .FirstOrDefaultAsync();

        if (attendance == null)
        {
            _logger.LogError($"Most recent attendance for user with ID {userId} was not found");
            return null;
        }
        
        return attendance;
    }

    public async Task<AttendanceCheckEntity?> GetAttendanceCheckByIdAsync(int id, string uniId)
    {
        var result = await _context.AttendanceChecks.FirstOrDefaultAsync(ca => ca.Id == id);
        if (result == null)
        {
            _logger.LogError($"AttendanceCheck with ID {id} was not found");
            return null;
        }
        
        var accessible = await IsAttendanceCheckAccessibleByUser(result, uniId);
        if (!accessible)
        {
            _logger.LogError($"AttendanceCheck with ID {id} cannot be fetched");
            return null;
        }
        
       return result;
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
                StartTime = date.ToDateTime(startTime).ToUniversalTime(),
                EndTime = date.ToDateTime(endTime).ToUniversalTime(),
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

    public async Task<bool> DeleteAttendance(int id, string uniId)
    {
        var attendance = await GetCourseAttendanceByIdAsync(id, uniId);
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
    
    public async Task<bool> DeleteAttendanceCheck(int id, string uniId)
    {
        var attendanceCheck = await GetAttendanceCheckByIdAsync(id, uniId);
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

    public async Task<bool> IsAttendanceAccessibleByUser(CourseAttendanceEntity attendance, string uniId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UniId == uniId);
        var result = await _context.CourseTeachers
            .CountAsync(ct => ct.TeacherId == user!.Id && ct.CourseId == attendance.CourseId);

        if (result <= 0)
        {
            _logger.LogError($"Attendance with ID {attendance.Id} is not accessible by user with UNI-ID {uniId}");
            return false;
        }
        
        return true;
    }
    
    public async Task<bool> IsAttendanceCheckAccessibleByUser(AttendanceCheckEntity attendanceCheck, string uniId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UniId == uniId);
        var attendance = await _context.CourseAttendances.FirstOrDefaultAsync(at => at.Id == 
                                                                attendanceCheck.CourseAttendanceId);
        var result = await _context.CourseTeachers
            .CountAsync(ct => ct.TeacherId == user!.Id && ct.CourseId == attendance!.CourseId);
        
        if (result <= 0)
        {
            _logger.LogError($"Attendance check with ID {attendanceCheck.Id} is not accessible by user with UNI-ID {uniId}");
            return false;
        }
        
        return true;
    }
}