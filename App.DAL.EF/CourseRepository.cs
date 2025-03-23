using App.Domain;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF;

public class CourseRepository(AppDbContext context)
{
   public async Task<List<CourseEntity>> GetCoursesByUser(int id)
    {
        return await context.Courses
            .Where(ca => ca.CourseTeacherEntities!
                .Any(ct => ct.TeacherId == id))
            .ToListAsync();
    }
    
    public async Task AddCourseEntity(CourseTeacherEntity teacher, CourseEntity newCourse)
    {
        teacher.CreatedAt = DateTime.Now.ToUniversalTime();
        teacher.UpdatedAt = DateTime.Now.ToUniversalTime();
        newCourse.CreatedAt = DateTime.Now.ToUniversalTime();
        newCourse.UpdatedAt = DateTime.Now.ToUniversalTime();
        
        await context.Courses.AddAsync(newCourse);
        await context.CourseTeachers.AddAsync(teacher);
        await context.SaveChangesAsync();
    }
    
    public async Task<bool> UpdateCourseEntity(int courseId, CourseEntity updatedCourse)
    {
        var course = await context.Courses.FirstOrDefaultAsync(u => u.Id == courseId);

        if (course == null)
        {
            return false;
        }
        
        course.CourseName = updatedCourse.CourseName;
        course.CourseCode = updatedCourse.CourseCode;
        course.CourseValidStatus = updatedCourse.CourseValidStatus;
 
        course.UpdatedAt = DateTime.Now.ToUniversalTime();
        await context.SaveChangesAsync();
        return true;
    }
    
    public async Task DeleteCourseEntity(CourseEntity course)
    {
        context.Courses.Remove(course);
        await context.SaveChangesAsync();
    }
    
    public async Task<List<CourseUserCountDto>?> GetAllUserCountsByCourseId(int courseId)
    {
        var course = await context.Courses.FirstOrDefaultAsync(u => u.Id == courseId);

        if (course == null)
        {
            return null;
        }
        
        var attendanceCounts = await context.CourseAttendances
            .Where(a => a.CourseId == courseId)
            .Select(a => new CourseUserCountDto
            {
                AttendanceDate = a.StartTime.Date,
                UserCount = a.AttendanceChecks!.Count()
            })
            .ToListAsync();

        return attendanceCounts;
    }
    

}