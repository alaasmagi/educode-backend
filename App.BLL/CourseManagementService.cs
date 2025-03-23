using System.Runtime.InteropServices.JavaScript;
using App.DAL.EF;
using App.Domain;
using Contracts;
using Microsoft.EntityFrameworkCore;

namespace App.BLL;

public class CourseManagementService : ICourseManagementService
{
    private readonly AppDbContext _context;
    private readonly CourseRepo courseRepo;
    private readonly AttendanceRepo attendanceRepo;

    public CourseManagementService(AppDbContext context)
    {
        _context = context;
        courseRepo = new CourseRepo(_context); 
        attendanceRepo = new AttendanceRepo(_context); 
    }

    public async Task<CourseEntity?> GetCourseByAttendanceIdAsync(int attendanceId)
    {
        var courseAttendance = await attendanceRepo.GetAttendance(attendanceId);

        if (courseAttendance == null)
        {
            return null;
        }
        
        var course = await courseRepo.GetCourse(courseAttendance.CourseId);
        return course ?? null;
    }

    public async Task<CourseEntity?> GetCourseByIdAsync(int courseId)
    {
        return await courseRepo.GetCourse(courseId) ?? null;
    }
    
    public async Task<bool> AddCourse(UserEntity user, CourseEntity course, string creator)
    {
        CourseTeacherEntity courseTeacher = new CourseTeacherEntity()
        {
            CourseId = course.Id,
            Course = course,
            Teacher = user,
            TeacherId = user.Id,
            CreatedBy = creator,
            UpdatedBy = creator
        };
        if (await DoesCourseExistAsync(course.Id))
        {
            return false;
        }

        await courseRepo.AddCourseEntity(courseTeacher, course);
        return true;
    }
    public async Task<bool> EditCourse(int courseId, CourseEntity newCourse)
    {
        if (!await DoesCourseExistAsync(courseId))
        {
            return false;
        }
        
        await courseRepo.UpdateCourseEntity(courseId, newCourse);
        return true;
    }
    public async Task<bool> DeleteCourse(int id)
    {
        var course = await GetCourseByIdAsync(id);
        if (course == null)
        {
            return false;
        }
        
        await courseRepo.DeleteCourseEntity(course);
        return true;
    }
    
    public List<CourseStatusDto> GetAllCourseStatuses()
    {
        return Enum.GetValues(typeof(ECourseValidStatus))
            .Cast<ECourseValidStatus>()
            .Select(status => new CourseStatusDto
            {
                Id = (int)status,
                Status = status.ToString().ToLower()
            })
            .ToList();
    }
    
    public async Task<List<CourseEntity>> GetCoursesByUserAsync(string uniId)
    {
        return await courseRepo.GetCoursesByUser(uniId);
    }
    
    public async Task<List<CourseUserCountDto>?> GetAttendancesUserCountsByCourseAsync(int courseId)
    {
        return await courseRepo.GetAllUserCountsByCourseId(courseId) ?? null;
    }
    
    public async Task<bool> DoesCourseExistAsync(int id)
    {
        return await _context.Courses.AnyAsync(u => u.Id == id);
    }
}
   
    