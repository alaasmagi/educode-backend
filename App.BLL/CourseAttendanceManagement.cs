using System.Runtime.InteropServices.JavaScript;
using App.DAL.EF;
using App.Domain;

namespace App.BLL;

public class CourseAttendanceManagement
{
    private readonly AppDbContext _context;
    private readonly CourseAttendanceRepository CourseAttendance;

    public CourseAttendanceManagement(AppDbContext context)
    {
        _context = context;
        CourseAttendance = new CourseAttendanceRepository(_context); 
    }

    public async Task<CourseEntity?> GetCourseByAttendanceId(int attendanceId)
    {
        var courseAttendance = await CourseAttendance.GetAttendance(attendanceId);

        if (courseAttendance == null)
        {
            return null;
        }
        
        var course = await CourseAttendance.GetCourse(courseAttendance.CourseId);

        return course ?? null;
    }

    public async Task<CourseAttendanceEntity?> GetCourseAttendanceById(int attendanceId)
    {
        var courseAttendance = await CourseAttendance.GetAttendance(attendanceId);

        return courseAttendance ?? null;
    }

    public async Task AddAttendanceCheck(AttendanceCheckEntity attendanceCheck, string creator)
    {
        await CourseAttendance.AddAttendanceCheck(attendanceCheck, creator);
    }

    public async Task<CourseEntity?> GetCurrentAttendanceCourse(UserEntity user)
    {
        var currentCourse = await CourseAttendance.GetCurrentAttendance(user.Id);
        return currentCourse ?? null;
    }
}
   
    