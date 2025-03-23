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
}