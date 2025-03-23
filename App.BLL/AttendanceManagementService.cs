using App.DAL.EF;
using App.Domain;
using Contracts;
using Microsoft.EntityFrameworkCore;

namespace App.BLL;

public class AttendanceManagementService : IAttendanceManagementService
{
    private readonly AppDbContext _context;
    private readonly AttendanceRepository _attendanceRepository;

    public AttendanceManagementService(AppDbContext context)
    {
        _context = context;
        _attendanceRepository = new AttendanceRepository(_context); 
    }

    private async Task<bool> DoesAttendanceExist(int id)
    { 
        return await _context.CourseAttendances.AnyAsync(u => u.Id == id);
    }
    
    public async Task<CourseAttendanceEntity?> GetCurrentAttendanceAsync(int userId)
    {
        var currentAttendance = await _attendanceRepository.GetCurrentAttendance(userId);
        return currentAttendance ?? null;
    }
    
    public async Task<CourseAttendanceEntity?> GetCourseAttendanceByIdAsync(int attendanceId)
    {
        var courseAttendance = await _context.CourseAttendances
            .FirstOrDefaultAsync(u => u.Id == attendanceId);

        return courseAttendance ?? null;
    }
    
    public async Task<List<CourseAttendanceEntity>?> GetAttendancesByCourseAsync(int courseId)
    {
        var attendances = await _context.CourseAttendances
            .Where(c => c.CourseId == courseId).ToListAsync();
        return attendances.Count > 0 ? attendances : null;
    }

    public async Task AddAttendanceCheckAsync(AttendanceCheckEntity attendanceCheck, string creator)
    {
        await _attendanceRepository.AddAttendanceCheck(attendanceCheck, creator);
    }

    public async Task<List<AttendanceCheckEntity>?> GetAttendanceChecksByAttendanceIdAsync(int attendanceId)
    {
        var attendanceChecks = await _context.AttendanceChecks
            .Where(c => c.CourseAttendanceId == attendanceId).ToListAsync();
        return attendanceChecks.Count > 0 ? attendanceChecks : null;
    }

    public async Task<CourseAttendanceEntity?> GetMostRecentAttendanceByUserAsync(int userId)
    {
        var attendance = await _context.CourseAttendances
            .Where(ca => ca.Course!.CourseTeacherEntities!
                .Any(ct => ct.TeacherId == userId))
            .OrderByDescending(ca => ca.EndTime)
            .FirstOrDefaultAsync();

        return attendance ?? null;
    }

    public async Task<AttendanceCheckEntity?> GetAttendanceCheckByIdAsync(int id)
    {
        return await _context.AttendanceChecks.FirstOrDefaultAsync(ca => ca.Id == id);
    }
    
    public async Task<List<AttendanceTypeEntity>> GetAttendanceTypesAsync()
    {
        return await _context.AttendanceTypes.ToListAsync();
    }

    public async Task<AttendanceTypeEntity?> GetAttendanceTypeByIdAsync(int attendanceTypeId)
    {
        var result = await _context.AttendanceTypes
            .FirstOrDefaultAsync(ca => ca.Id == attendanceTypeId);
        return result ?? null;
    }
    
    public async Task AddAttendanceAsync(CourseAttendanceEntity newAttendance, List<DateOnly> attendanceDates, 
                                                                                TimeOnly startTime, TimeOnly endTime)
    {
        foreach (var date in attendanceDates)
        {
            newAttendance.StartTime = date.ToDateTime(startTime);
            newAttendance.EndTime = date.ToDateTime(endTime);
        }
        
        await _attendanceRepository.AddAttendance(newAttendance);
    }

    public async Task<bool> EditAttendanceAsync(int attendanceId, CourseAttendanceEntity updatedAttendance)
    {
        if (!await DoesAttendanceExist(attendanceId))
        {
            return false;
        }
        
        var status = await _attendanceRepository.UpdateAttendance(attendanceId, updatedAttendance);
        return status;
    }
    
    public async Task<bool> DeleteAttendance(int id)
    {
        var attendance = await GetCourseAttendanceByIdAsync(id);
        if (attendance == null)
        {
            return false;
        }
        
        await _attendanceRepository.DeleteAttendanceEntity(attendance);
        return true;
    }
    
    public async Task<bool> DeleteAttendanceCheck(int id)
    {
        var attendanceCheck = await GetAttendanceCheckByIdAsync(id);
        if (attendanceCheck == null)
        {
            return false;
        }
        
        await _attendanceRepository.DeleteAttendanceCheckEntity(attendanceCheck);
        return true;
    }
}