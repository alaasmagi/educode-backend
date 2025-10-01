using App.BLL;
using App.Domain;
using App.DTO;
using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.ApiControllers;

[ApiController]
[Route("api/[controller]")]
public class AttendanceCheckController(
    IAttendanceManagementService attendanceManagementService,
    ICourseManagementService courseManagementService,
    IUserManagementService userManagementService,
    ILogger<AttendanceCheckController> logger)
    : ControllerBase
{
    [Authorize(Policy = nameof(EAccessLevel.TertiaryLevel))]
    [HttpGet("{id}")]
    public async Task<ActionResult<IEnumerable<AttendanceCheckDto>>> GetAttendanceCheckById(Guid id)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        var userId = User.FindFirst(Constants.UserIdClaim)?.Value ?? string.Empty;
        var courseAttendance = await attendanceManagementService.GetCourseAttendanceByIdAsync(id, userId);

        if (courseAttendance == null)
        {
            return NotFound(new {message = "Attendance not found", messageCode = "attendance-not-found"});
        }
        
        var attendanceChecks = 
            await attendanceManagementService.GetAttendanceChecksByAttendanceIdAsync(courseAttendance.Identifier);
        if (attendanceChecks == null)
        {
            return Ok(new {message = "Attendance has no attendance checks", messageCode = "attendance-has-no-checks"});
        }
        
        var result = AttendanceCheckDto.ToDtoList(attendanceChecks);
        
        logger.LogInformation($"Attendance checks for attendance with ID {id} successfully fetched");
        return Ok(result);
    }
    
    [Authorize(Policy = nameof(EAccessLevel.PrimaryLevel))]
    [HttpPost]
    public async Task<IActionResult> AddAttendanceCheck([FromBody] AttendanceCheckModel model)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        if (!ModelState.IsValid)
        {
            logger.LogWarning($"Form data is invalid");
            return BadRequest(new {message = "Invalid credentials", messageCode = "invalid-credentials"});
        }
        
        var newAttendanceCheck = new AttendanceCheckEntity
        {
            StudentCode = model.StudentCode,
            FullName = model.FullName,
            AttendanceIdentifier = model.CourseAttendanceIdentifier,
            CreatedBy = model.Creator,
            UpdatedBy = model.Creator,
        };

        if (model.WorkplaceIdentifier != null)
        {
            string workplaceIdentifier = model.WorkplaceIdentifier;
            if(!await attendanceManagementService.DoesWorkplaceExist(workplaceIdentifier))
            {
                return NotFound(new {message = "Workplace was not found ", messageCode = "workplace-not-found"});
            }
        }

        if (!await attendanceManagementService.AddAttendanceCheckAsync(newAttendanceCheck, model.Creator, model.WorkplaceIdentifier ?? null))
        {
            return BadRequest(new {message = "Attendance check already exists", 
                messageCode = "attendance-check-already-exists" });
        }

        logger.LogInformation($"Attendance check added successfully");
        return Ok();
    }
    
    [Authorize(Policy = nameof(EAccessLevel.TertiaryLevel))]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAttendanceCheck(Guid id)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        var userId = User.FindFirst(Constants.UserIdClaim)?.Value ?? string.Empty;
        if (!ModelState.IsValid)
        {
            logger.LogWarning($"Form data is invalid");
            return BadRequest(new { message = "Invalid credentials", messageCode = "invalid-credentials" });
        }

        if (!await attendanceManagementService.DeleteAttendanceCheck(id, userId))
        {
            return BadRequest(new { message = "AttendanceCheck does not exist", 
                messageCode = "attendance-check-does-not-exist" });
        }
        
        logger.LogInformation($"Attendance check with ID {id} deleted successfully");
        return Ok();
    }
}