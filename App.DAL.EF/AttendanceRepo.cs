using App.Domain;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF;

public class AttendanceRepo(AppDbContext context)
{
    public async Task<CourseAttendanceEntity?> GetAttendance(int attendanceId)
    {
        return await context.CourseAttendances.FirstOrDefaultAsync(u => u.Id == attendanceId);;
    }

    public async Task AddAttendanceCheck(AttendanceCheckEntity attendance, string creator)
    {
        attendance.CreatedBy = creator;
        attendance.UpdatedBy = creator;
        attendance.CreatedAt = DateTime.Now.ToUniversalTime();
        attendance.UpdatedAt= DateTime.Now.ToUniversalTime();
        
        await context.AttendanceChecks.AddAsync(attendance);
        await context.SaveChangesAsync();
    }

    public async Task<CourseAttendanceEntity?> GetCurrentAttendance(int userId)
    {
        var ongoingAttendance= await context.CourseAttendances
            .Where(ca => ca.StartTime <= DateTime.Now && ca.EndTime >= DateTime.Now &&
                         ca.Course!.CourseTeacherEntities!.Any(ct => ct.TeacherId == userId)).
            Include(ca => ca.Course)
            .FirstOrDefaultAsync() ?? null;
        
        return ongoingAttendance;
    }
}