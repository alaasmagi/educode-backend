using App.Domain;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF;

public class CourseRepository(AppDbContext context)
{
   public async Task<List<CourseEntity>?> GetCoursesByUser(int id)
    {
        var result = await context.Courses
            .Where(ca => ca.CourseTeacherEntities!
                .Any(ct => ct.TeacherId == id)).ToListAsync();
        
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
        updatedCourse.UpdatedAt = DateTime.Now.ToUniversalTime();
        return await context.Courses
            .Where(c => c.Id == courseId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(c => c.CourseName, updatedCourse.CourseName)
                .SetProperty(c => c.CourseCode, updatedCourse.CourseCode)
                .SetProperty(c => c.CourseValidStatus, updatedCourse.CourseValidStatus)
                .SetProperty(c => c.UpdatedAt, updatedCourse.UpdatedAt)
            ) > 0;
    }
    
    public async Task<bool> DeleteCourseEntity(CourseEntity course)
    {
        context.Courses.Remove(course);
        return await context.SaveChangesAsync() > 0;
    }
    
    public async Task<List<CourseUserCountDto>?> GetAllUserCountsByCourseId(int courseId)
    {
        var courseExists = await context.Courses.AnyAsync(c => c.Id == courseId);
        if (!courseExists)
            return null;

        var attendances = await context.CourseAttendances
            .Where(ca => ca.CourseId == courseId)
            .ToListAsync();

        var result = new List<CourseUserCountDto>();

        foreach (var attendance in attendances)
        {
            var count = await context.AttendanceChecks
                .CountAsync(ac => ac.CourseAttendanceId == attendance.Id);

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
        return await context.Courses.AnyAsync(c => c.CourseCode == courseCode);
    }
    
    public async Task<bool> CourseAvailabilityCheckById(int id)
    {
        return await context.Courses.AnyAsync(c => c.Id == id);
    }
    
    public async Task<CourseEntity?> GetCourseById(int courseId)
    {
        return await context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
    }
    
    public async Task<CourseEntity?> GetCourseByName(string courseName)
    {
        return await context.Courses.FirstOrDefaultAsync(c => c.CourseName == courseName);
    }
    
    public async Task<CourseEntity?> GetCourseByCode(string courseCode)
    {
        return await context.Courses.FirstOrDefaultAsync(c => c.CourseCode == courseCode);
    }

    public async Task<int> CourseAccessibilityCheck(int courseId, int userId)
    {
        return await context.CourseTeachers
            .CountAsync(ct => ct.TeacherId == userId && ct.CourseId == courseId);
    }

    public async Task<bool> CourseOnlyTeacherCheck(int userId, int courseId)
    {
        var courseTeachers = await context.CourseTeachers.Where(c => c.CourseId == courseId).ToListAsync();

        if (courseTeachers.Count == 1 && courseTeachers[0].TeacherId == userId)
        {
            return true;
        }

        return false;
    }
    
    public async Task<bool> RemoveOldCourses(DateTime datePeriod)
    {
        var oldCourses = await context.Courses
            .Where(u => u.UpdatedAt < datePeriod && u.Deleted == true)
            .ToListAsync();

        if (!oldCourses.Any())
        {
            return false;
        }

        context.Courses.RemoveRange(oldCourses);
        await context.SaveChangesAsync();
        
        return true;
    }
    
    public async Task<bool> RemoveOldCourseTeachers(DateTime datePeriod)
    {
        var oldCourseTeachers = await context.CourseTeachers
            .Where(u => u.UpdatedAt < datePeriod && u.Deleted == true)
            .ToListAsync();

        if (!oldCourseTeachers.Any())
        {
            return false;
        }

        context.CourseTeachers.RemoveRange(oldCourseTeachers);
        await context.SaveChangesAsync();
        
        return true;
    }
}