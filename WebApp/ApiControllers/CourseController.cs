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
    private readonly UserManagement userManagement;

    public CourseController(AppDbContext context, IConfiguration configuration)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        var config = configuration ?? throw new ArgumentNullException(nameof(configuration));
    
        courseManagement = new CourseAttendanceManagement(_context);
        userManagement = new UserManagement(_context);
    }
    
    [Authorize]
    [HttpGet("Course/AttendanceId/{id}")]
    public async Task<ActionResult<CourseEntity>> GetCourseByAttendanceId(int id)
    {
        var courseEntity = await courseManagement.GetCourseByAttendanceId(id);

        if (courseEntity == null)
        {
            return NotFound();
        }

        return courseEntity;
    }
    
    [Authorize]
    [HttpGet("Attendance/Id/{id}")]
    public async Task<ActionResult<CourseAttendanceEntity>> GetAttendanceById(int id)
    {
        var attendanceEntity = await courseManagement.GetCourseAttendanceById(id);

        if (attendanceEntity == null)
        {
            return NotFound();
        }

        return attendanceEntity;
    }

    [Authorize]
    [HttpPost("GetCurrentAttendanceCourse")]
    public async Task<ActionResult<CourseAttendanceEntity>> GetCurrenAttendance(string uniId)
    {
        var user = await userManagement.GetUserByUniId(uniId);

        if (user == null)
        {
            return BadRequest();
        }
        
        var courseAttendanceEntity = await courseManagement.GetCurrentAttendance(user);

        if (courseAttendanceEntity?.Course == null)
        {
            return Ok(new { message = "No course found" });
        }

        var returnEntity = new CurrentLectureReturnModel
        {
            CourseCode = courseAttendanceEntity.Course.CourseCode,
            CourseName = courseAttendanceEntity.Course.CourseName,
            AttendanceId = courseAttendanceEntity.Id
        };
        
        return Ok(returnEntity);
    }
    
    [Authorize]
    [HttpPost("AttendanceCheck/Add")]
    public async Task<IActionResult> AddAttendanceCheck([FromBody] AttendanceCheckModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        
        AttendanceCheckEntity newAttendanceCheck = new AttendanceCheckEntity()
        {
           StudentCode = model.StudentCode,
           CourseAttendanceId = model.CourseAttendanceId,
           WorkplaceId = model.WorkplaceId ?? null
        };
        
        await courseManagement.AddAttendanceCheck(newAttendanceCheck, model.Creator);

        return Ok();
    }
}