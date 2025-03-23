using App.Domain;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF;

public class CourseRepository(AppDbContext context)
{
   public async Task<List<CourseEntity>?> GetCoursesByUser(int id)
    {
        var result = await context.Courses
            .Where(ca => ca.CourseTeacherEntities!
                .Any(ct => ct.TeacherId == id))
            .ToListAsync();
        
        return result.Count > 0 ? result : null; 
    }
    
    public async Task<bool> AddCourseEntity(CourseTeacherEntity teacher, CourseEntity newCourse)
    {
        newCourse.CreatedAt = DateTime.Now.ToUniversalTime();
        newCourse.UpdatedAt = DateTime.Now.ToUniversalTime();
        
        await context.Courses.AddAsync(newCourse);
        
        teacher.CourseId = newCourse.Id;
        teacher.Course = newCourse;
        teacher.CreatedAt = DateTime.Now.ToUniversalTime();
        teacher.UpdatedAt = DateTime.Now.ToUniversalTime();
        
        await context.CourseTeachers.AddAsync(teacher);
        return await context.SaveChangesAsync() > 0;
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
        return await context.SaveChangesAsync() > 0;
    }
    
    public async Task<bool> DeleteCourseEntity(CourseEntity course)
    {
        context.Courses.Remove(course);
        return await context.SaveChangesAsync() > 0;
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

        return attendanceCounts.Count > 0 ? attendanceCounts : null;
    }
    

}