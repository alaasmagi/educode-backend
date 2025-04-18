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
            .AsNoTracking().ToListAsync();
        
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
        var courseExists = await context.Courses.AsNoTracking().AnyAsync(c => c.Id == courseId);
        if (!courseExists)
            return null;

        var attendances = await context.CourseAttendances
            .Where(ca => ca.CourseId == courseId)
            .AsNoTracking()
            .ToListAsync();

        var result = new List<CourseUserCountDto>();

        foreach (var attendance in attendances)
        {
            var count = await context.AttendanceChecks
                .AsNoTracking().CountAsync(ac => ac.CourseAttendanceId == attendance.Id);

            result.Add(new CourseUserCountDto
            {
                AttendanceDate = attendance.StartTime,
                UserCount = count
            });
        }

        return result;
    }

    public async Task<bool> CourseAvailabilityCheckByCourseCode(string courseCode)
    {
        return await context.Courses.AsNoTracking().AnyAsync(c => c.CourseCode == courseCode);
    }
    
    public async Task<bool> CourseAvailabilityCheckById(int id)
    {
        return await context.Courses.AsNoTracking().AnyAsync(c => c.Id == id);
    }
    
    public async Task<CourseEntity?> GetCourseById(int courseId)
    {
        return await context.Courses.AsNoTracking().FirstOrDefaultAsync(c => c.Id == courseId);
    }
    
    public async Task<CourseEntity?> GetCourseByName(string courseName)
    {
        return await context.Courses.AsNoTracking().FirstOrDefaultAsync(c => c.CourseName == courseName);
    }
    
    public async Task<CourseEntity?> GetCourseByCode(string courseCode)
    {
        return await context.Courses.AsNoTracking().FirstOrDefaultAsync(c => c.CourseCode == courseCode);
    }

    public async Task<int> CourseAccessibilityCheck(int courseId, int userId)
    {
        return await context.CourseTeachers
            .AsNoTracking()
            .CountAsync(ct => ct.TeacherId == userId && ct.CourseId == courseId);
    }
}