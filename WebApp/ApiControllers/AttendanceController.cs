using App.Domain;
using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.ApiControllers;

[ApiController]
[Route("api/[controller]")]
public class AttendanceController(
    IAttendanceManagementService attendanceManagementService,
    IUserManagementService userManagementService)
    : ControllerBase
{
    [Authorize]
    [HttpGet("Id/{id}")]
    public async Task<ActionResult<CourseAttendanceEntity>> GetAttendanceById(int id)
    {
        var attendanceEntity = await attendanceManagementService.GetCourseAttendanceByIdAsync(id);

        if (attendanceEntity == null)
        {
            return NotFound(new {message = "Attendance not found", error = "attendance-not-found"});
        }

        return attendanceEntity;
    }

    [Authorize]
    [HttpGet("GetCurrentAttendance/UniId/{uniId}")]
    public async Task<ActionResult<CourseAttendanceEntity>> GetCurrenAttendance(string uniId)
    {
        var user = await userManagementService.GetUserByUniIdAsync(uniId);

        if (user == null)
        {
            return NotFound(new {message = "User not found", error = "user-not-found"});
        }
        
        var courseAttendanceEntity = await attendanceManagementService.GetCurrentAttendanceAsync(user);

        if (courseAttendanceEntity?.Course == null)
        {
            return Ok(new {message = "Current attendance not found", error = "current-attendance-not-found"});
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
            return BadRequest(new {message = "Invalid credentials", error = "invalid-credentials"});
        }

        var newAttendanceCheck = new AttendanceCheckEntity
        {
           StudentCode = model.StudentCode,
           CourseAttendanceId = model.CourseAttendanceId,
           WorkplaceId = model.WorkplaceId ?? null
        };
        
        await attendanceManagementService.AddAttendanceCheckAsync(newAttendanceCheck, model.Creator);

        return Ok();
    }
}