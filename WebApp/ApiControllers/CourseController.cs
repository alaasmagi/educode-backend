using App.BLL;
using App.DAL.EF;
using App.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.ApiControllers;

[ApiController]
[Route("api/[controller]")]
public class CourseController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly CourseAttendanceManagement courseManagement;

    public CourseController(AppDbContext context, IConfiguration configuration)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        var config = configuration ?? throw new ArgumentNullException(nameof(configuration));
    
        courseManagement = new CourseAttendanceManagement(_context);
    }
    
    
    [HttpGet("Course/AttendanceId/{id}")]
    public async Task<ActionResult<CourseEntity>> GetCourseByAttendance(int id)
    {
        var courseEntity = await courseManagement.GetCourseByAttendanceId(id);

        if (courseEntity == null)
        {
            return NotFound();
        }

        return courseEntity;
    }
}