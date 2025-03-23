using App.Domain;
using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.ApiControllers;

[ApiController]
[Route("api/[controller]")]
public class CourseController(
    ICourseManagementService courseManagementService,
    IUserManagementService userManagementService)
    : ControllerBase
{
    
    [Authorize(Roles = "Teacher")]
    [HttpGet("Id/{id}")]
    public async Task<ActionResult<CourseEntity>> GetCourseDetails(int id)
    {
        var courseEntity = await courseManagementService.GetCourseByIdAsync(id);

        if (courseEntity == null)
        {
            return NotFound(new {message = "Course not found", error = "course-not-found"});
        }

        return courseEntity;
    }
    
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
    
    [Authorize(Roles = "Teacher")]
    [HttpGet("Statuses")]
    public async Task<IEnumerable<ECourseValidStatus>> GetAllCourseStatuses()
    {
        var courseStatuses = courseManagementService.GetAllCourseStatuses();
        return courseStatuses;
    }
    
    [Authorize(Roles = "Teacher")]
    [HttpGet("UniId/{uniId}")]
    public async Task<ActionResult<IEnumerable<CourseEntity>>> GetAllCoursesByUser(string uniId)
    {
        var validity = await userManagementService.DoesUserExistAsync(uniId);
        if(!validity)
        {
            return NotFound(new {message = "User not found", error = "user-not-found"});
            
        }
        var result = await courseManagementService.GetCoursesByUserAsync(uniId);
        return Ok(result);
    }
    
    [Authorize(Roles = "Teacher")]
    [HttpGet("StudentCounts/{id}")]
    public async Task<ActionResult<IEnumerable<CourseUserCountDto>>> GetAllStudentCountsByCourse(int id)
    {
        var validity = await courseManagementService.DoesCourseExistAsync(id);
        if(!validity)
        {
            return NotFound(new {message = "Course not found", error = "course-not-found"});
        }
        
        var result = await courseManagementService.GetAttendancesUserCountsByCourseAsync(id);
        return Ok(result);
    }
    
    [Authorize(Roles = "Teacher")]
    [HttpPost("Add")]
    public async Task<ActionResult<CourseEntity>> AddCourse([FromBody] CourseModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { message = "Invalid credentials", error = "invalid-credentials" });
        }

        var user = await userManagementService.GetUserByUniIdAsync(model.UniId);
        if (user == null)
        {
            return BadRequest(new { message = "User does not exist", error = "user-not-found" });
        }
        
        var newCourse = new CourseEntity
        {
            CourseName = model.CourseName,
            CourseCode = model.CourseCode,
            CourseValidStatus = model.Status,
            CreatedBy = model.Creator,
            UpdatedBy = model.Creator
        };

        if (!await courseManagementService.AddCourse(user, newCourse, model.Creator))
        {
            return BadRequest(new { message = "Course already exists", error = "course-already-exists" });
        }

        return Ok();
    }
    
    [Authorize(Roles = "Teacher")]
    [HttpPatch("Edit")]
    public async Task<ActionResult<CourseEntity>> EditCourse([FromBody] CourseModel model)
    {
        if (!ModelState.IsValid || model.Id == null)
        {
            return BadRequest(new { message = "Invalid credentials", error = "invalid-credentials" });
        }
        
        var newCourse = new CourseEntity
        {
            CourseName = model.CourseName,
            CourseCode = model.CourseCode,
            CourseValidStatus = model.Status,
            CreatedBy = model.Creator,
            UpdatedBy = model.Creator
        };

        int courseId = model.Id ?? 0;
        if (!await courseManagementService.EditCourse(courseId, newCourse))
        {
            return BadRequest(new { message = "Course does not exist", error = "course-does-not-exist" });
        }

        return Ok();
    }
    
    [Authorize(Roles = "Teacher")]
    [HttpDelete("Delete/{id}")]
    public async Task<ActionResult<CourseEntity>> DeleteCourse(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { message = "Invalid credentials", error = "invalid-credentials" });
        }
        
        if (!await courseManagementService.DeleteCourse(id))
        {
            return BadRequest(new { message = "Course does not exist", error = "course-does-not-exist" });
        }

        return Ok();
    }
}