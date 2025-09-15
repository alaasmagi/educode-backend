using App.Domain;
using App.DTO;
using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.ApiControllers;

[ApiController]
[Route("api/[controller]")]
public class SchoolController(
    IAttendanceManagementService attendanceManagementService,
    ICourseManagementService courseManagementService,
    IUserManagementService userManagementService,
    ILogger<OtpController> logger)
    : ControllerBase
{
    [Authorize(Policy = nameof(EAccessLevel.SecondaryLevel))]
    [HttpGet("{id}")]
    public async Task<ActionResult<CourseAttendanceDto>> GetAttendanceById(Guid id)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        var userId = User.FindFirst(Constants.UserIdClaim)?.Value ?? string.Empty;
        var attendanceEntity = await attendanceManagementService.GetCourseAttendanceByIdAsync(id, userId);

        if (attendanceEntity == null)
        {
            return NotFound(new { message = "Attendance not found", messageCode = "attendance-not-found" });
        }

        var result = new CourseAttendanceDto(attendanceEntity);

        logger.LogInformation($"Attendance with ID {id} successfully fetched");
        return result;
    }
}