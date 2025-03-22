using System.Runtime.InteropServices.JavaScript;
using App.DAL.EF;
using App.Domain;
using Contracts;

namespace App.BLL;

public class CourseManagementService : ICourseManagementService
{
    private readonly AppDbContext _context;
    private readonly CourseAttendanceRepository CourseAttendance;

    public CourseManagementService(AppDbContext context)
    {
        _context = context;
        CourseAttendance = new CourseAttendanceRepository(_context); 
    }

    public async Task<CourseEntity?> GetCourseByAttendanceIdAsync(int attendanceId)
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
   
    