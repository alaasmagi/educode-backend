using App.DAL.EF;
using App.Domain;
using Contracts;

namespace App.BLL;

public class AttendanceManagementService : IAttendanceManagementService
{
    private readonly AppDbContext _context;
    private readonly AttendanceRepo attendanceRepo;

    public AttendanceManagementService(AppDbContext context)
    {
        _context = context;
        attendanceRepo = new AttendanceRepo(_context); 
    }
    
    public async Task<CourseAttendanceEntity?> GetCurrentAttendanceAsync(UserEntity user)
    {
        var currentAttendance = await attendanceRepo.GetCurrentAttendance(user.Id);
        return currentAttendance ?? null;
    }
    
    public async Task<CourseAttendanceEntity?> GetCourseAttendanceByIdAsync(int attendanceId)
    {
        var courseAttendance = await attendanceRepo.GetAttendance(attendanceId);

        return courseAttendance ?? null;
    }

    public async Task AddAttendanceCheckAsync(AttendanceCheckEntity attendanceCheck, string creator)
    {
        await attendanceRepo.AddAttendanceCheck(attendanceCheck, creator);
    }
}