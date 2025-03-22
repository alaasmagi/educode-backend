using App.Domain;
using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.ApiControllers;

[ApiController]
[Route("api/[controller]")]
public class CourseController(
    ICourseManagementService courseManagementService,
    IUserManagementService userManagementService)
    : ControllerBase
{
    [Authorize]
    [HttpGet("AttendanceId/{id}")]
    public async Task<ActionResult<CourseEntity>> GetCourseByAttendanceId(int id)
    {
        var courseEntity = await courseManagementService.GetCourseByAttendanceIdAsync(id);

        if (courseEntity == null)
        {
            return NotFound(new {message = "Course not found", error = "course-not-found"});
        }

        return courseEntity;
    }
    
    
}