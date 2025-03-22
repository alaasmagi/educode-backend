using App.DAL.EF;
using App.Domain;
using Contracts;

namespace App.BLL;

public class AttendanceManagementService : IAttendanceManagementService
{
    private readonly AppDbContext _context;
    private readonly CourseAttendanceRepository CourseAttendance;

    public AttendanceManagementService(AppDbContext context)
    {
        _context = context;
        CourseAttendance = new CourseAttendanceRepository(_context); 
    }
    
    public async Task<CourseAttendanceEntity?> GetCurrentAttendanceAsync(UserEntity user)
    {
        var currentAttendance = await CourseAttendance.GetCurrentAttendance(user.Id);
        return currentAttendance ?? null;
    }
    
    public async Task<CourseAttendanceEntity?> GetCourseAttendanceByIdAsync(int attendanceId)
    {
        var courseAttendance = await CourseAttendance.GetAttendance(attendanceId);

        return courseAttendance ?? null;
    }

    public async Task AddAttendanceCheckAsync(AttendanceCheckEntity attendanceCheck, string creator)
    {
        await CourseAttendance.AddAttendanceCheck(attendanceCheck, creator);
    }
}