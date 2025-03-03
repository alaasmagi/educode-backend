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
        var CourseAttendance = new CourseAttendanceRepository(_context); 
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
}
   
    