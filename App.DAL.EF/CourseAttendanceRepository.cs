using App.Domain;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF;

public class CourseAttendanceRepository(AppDbContext context)
{
    public async Task<CourseEntity?> GetCourse(int courseId)
    {
        return await context.Courses.FirstOrDefaultAsync(u => u.Id == courseId);
    }

    public async Task<CourseAttendanceEntity?> GetAttendance(int attendanceId)
    {
        return await context.CourseAttendances.FirstOrDefaultAsync(u => u.Id == attendanceId);;
    }

    public async Task AddAttendanceCheck(AttendanceCheckEntity attendance, string creator)
    {
        attendance.CreatedBy = creator;
        attendance.UpdatedBy = creator;
        attendance.CreatedAt = DateTime.Now;
        attendance.UpdatedAt= DateTime.Now;
        
        await context.AttendanceChecks.AddAsync(attendance);
        await context.SaveChangesAsync();
    }
}