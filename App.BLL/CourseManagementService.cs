using System.Runtime.InteropServices.JavaScript;
using App.DAL.EF;
using App.Domain;
using Contracts;
using Microsoft.EntityFrameworkCore;

namespace App.BLL;

public class CourseManagementService : ICourseManagementService
{
    private readonly AppDbContext _context;
    private readonly CourseRepository _courseRepository;

    public CourseManagementService(AppDbContext context)
    {
        _context = context;
        _courseRepository = new CourseRepository(_context); 
    }

    public async Task<CourseEntity?> GetCourseByAttendanceIdAsync(int attendanceId)
    {
        var courseAttendance = await _context.CourseAttendances.FirstOrDefaultAsync(u => u.Id == attendanceId);

        if (courseAttendance == null)
        {
            return null;
        }
        
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseAttendance.CourseId);
        return course ?? null;
    }

    public async Task<CourseEntity?> GetCourseByIdAsync(int courseId)
    {
        return await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId) ?? null;
    }
    
    public async Task<CourseEntity?> GetCourseByNameAsync(string courseName)
    {
        return await _context.Courses.FirstOrDefaultAsync(c => c.CourseName == courseName) ?? null;
    }
    
    public async Task<CourseEntity?> GetCourseByCodeAsync(string courseCode)
    {
        return await _context.Courses.FirstOrDefaultAsync(c => c.CourseCode == courseCode) ?? null;
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

        await _courseRepository.AddCourseEntity(courseTeacher, course);
        return true;
    }
    public async Task<bool> EditCourse(int courseId, CourseEntity newCourse)
    {
        if (!await DoesCourseExistAsync(courseId))
        {
            return false;
        }
        
        await _courseRepository.UpdateCourseEntity(courseId, newCourse);
        return true;
    }
    public async Task<bool> DeleteCourse(int id)
    {
        var course = await GetCourseByIdAsync(id);
        if (course == null)
        {
            return false;
        }
        
        await _courseRepository.DeleteCourseEntity(course);
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
    
    public async Task<List<CourseEntity>> GetCoursesByUserAsync(int userId)
    {
        return await _courseRepository.GetCoursesByUser(userId);
    }
    
    public async Task<List<CourseUserCountDto>?> GetAttendancesUserCountsByCourseAsync(int courseId)
    {
        return await _courseRepository.GetAllUserCountsByCourseId(courseId) ?? null;
    }
    
    public async Task<bool> DoesCourseExistAsync(int id)
    {
        return await _context.Courses.AnyAsync(u => u.Id == id);
    }
}
   
    