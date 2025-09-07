using System.Security.Claims;
using App.Domain;
using App.DTO;
using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.ApiControllers;

[ApiController]
[Route("api/[controller]")]
public class CourseController(
    ICourseManagementService courseManagementService,
    IUserManagementService userManagementService,
    ILogger<CourseController> logger)
    : ControllerBase
{
    
    [Authorize(Policy = nameof(EAccessLevel.TertiaryLevel))]
    [HttpGet("{id}")]
    public async Task<ActionResult<CourseDto>> GetCourseDetails(Guid id)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        var userId = User.FindFirst(Constants.UserIdClaim)?.Value ?? string.Empty;
        var courseEntity = await courseManagementService.GetCourseByIdAsync(id, userId!);

        if (courseEntity == null)
        {
            return NotFound(new {message = "Course not found", messageCode = "course-not-found"});
        }
        
        var result = new CourseDto(courseEntity);
        
        logger.LogInformation($"Successfully fetched course by course ID {id}");
        return Ok(result);
    }
    
    [Authorize(Policy = nameof(EAccessLevel.PrimaryLevel))]
    [HttpGet("Attendance/{id}")]
    public async Task<ActionResult<CourseDto>> GetCourseByAttendanceId(Guid id)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        var courseEntity = await courseManagementService.GetCourseByAttendanceIdAsync(id);

        if (courseEntity == null)
        {
            return NotFound(new {message = "Course not found", messageCode = "course-not-found"});
        }
        
        var result = new CourseDto(courseEntity);
        
        logger.LogInformation($"Successfully fetched course by attendance with ID {id}");
        return Ok(result);
    }
    
    [Authorize(Policy = nameof(EAccessLevel.TertiaryLevel))]
    [HttpGet("Statuses")]
    public async Task<IActionResult> GetAllCourseStatuses()
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        var courseStatuses = await courseManagementService.GetAllCourseStatuses();

        if (courseStatuses == null)
        {
            return NotFound(new {message = "Course statuses not found", messageCode = "course-statuses-not-found"});
        }
        
        var result = CourseStatusDto.ToDtoList(courseStatuses);
        
        logger.LogInformation($"All course statuses fetched successfully");
        return Ok(result);
    }
    
    [Authorize(Policy = nameof(EAccessLevel.TertiaryLevel))]
    [HttpGet("User/{Id}")]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetAllCoursesByUser(Guid id)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        var userId = User.FindFirst(Constants.UserIdClaim)?.Value ?? string.Empty;
        var user = await userManagementService.GetUserByIdAsync(id);
        if(user == null)
        {
            return NotFound(new {message = "User not found", messageCode = "user-not-found"});
        }
        
        var courses = await courseManagementService.GetCoursesByUserAsync(user.Id);
        
        if (courses == null)
        {
            return Ok(new {message = "No courses found", messageCode = "courses-not-found"});
        }
        
        var result = CourseDto.ToDtoList(courses);
        
        logger.LogInformation($"All courses for user with ID {id}");
        return Ok(result);
    }
    
    [Authorize(Policy = nameof(EAccessLevel.TertiaryLevel))]
    [HttpGet("StudentCounts/{id}")]
    public async Task<ActionResult<IEnumerable<AttendanceStudentCountDto>>> GetAllStudentCountsByCourse(Guid id)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        var validity = await courseManagementService.DoesCourseExistByIdAsync(id);
        if(!validity)
        {
            return NotFound(new {message = "Course not found", messageCode = "course-not-found"});
        }
        
        var result = await courseManagementService.GetAttendancesUserCountsByCourseAsync(id);

        if (result == null)
        {
            return NotFound(new {message = "No student counts found", messageCode = "student-counts-not-found"});
        }
        
        logger.LogInformation($"All student counts for course with ID {id}");
        return Ok(result);
    }
    
    [Authorize(Policy = nameof(EAccessLevel.TertiaryLevel))]
    [HttpPost]
    public async Task<ActionResult> AddCourse([FromBody] CourseModel model)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        var userId = User.FindFirst(Constants.UserIdClaim)?.Value ?? string.Empty;
        if (!ModelState.IsValid)
        {
            logger.LogWarning($"Form data is invalid");
            return BadRequest(new { message = "Invalid credentials", messageCode = "invalid-credentials" });
        }

        var user = await userManagementService.GetUserByIdAsync(Guid.Parse(userId));
        if (user == null)
        {
            return BadRequest(new { message = "User does not exist", messageCode = "user-not-found" });
        }
        
        var newCourse = new CourseEntity
        {
            CourseName = model.CourseName,
            CourseCode = model.CourseCode,
            CourseStatusId = model.CourseStatusId,
            CreatedBy = model.Creator,
            UpdatedBy = model.Creator,
        };

        if (!await courseManagementService.AddCourse(user, newCourse, model.Creator))
        {
            
            return BadRequest(new { message = "Course already exists", messageCode = "course-already-exists" });
        }
        
        logger.LogInformation($"Course added successfully");
        return Ok();
    }
    
    [Authorize(Policy = nameof(EAccessLevel.TertiaryLevel))]
    [HttpPatch]
    public async Task<ActionResult> EditCourse([FromBody] CourseModel model)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        if (!ModelState.IsValid || model.Id == null)
        {
            logger.LogWarning($"Form data is invalid");
            return BadRequest(new { message = "Invalid credentials", messageCode = "invalid-credentials" });
        }
        
        var newCourse = new CourseEntity
        {
            CourseName = model.CourseName,
            CourseCode = model.CourseCode,
            CourseStatusId = model.CourseStatusId,
            CreatedBy = model.Creator,
            UpdatedBy = model.Creator,
        };

        var courseId = model.Id.Value;
        if (!await courseManagementService.EditCourse(courseId, newCourse))
        {
            return BadRequest(new { message = "Course does not exist", messageCode = "course-does-not-exist" });
        }
        
        logger.LogInformation($"Course with ID {model.Id} updated successfully");
        return Ok();
    }
    
    [Authorize(Policy = nameof(EAccessLevel.TertiaryLevel))]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCourse(Guid id)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        var userId = User.FindFirst(Constants.UserIdClaim)?.Value ?? string.Empty;
        
        if (!ModelState.IsValid)
        {
            logger.LogWarning($"Form data is invalid");
            return BadRequest(new { message = "Invalid credentials", messageCode = "invalid-credentials" });
        }
        
        if (!await courseManagementService.DeleteCourse(id, userId!))
        {
            return BadRequest(new { message = "Course does not exist", messageCode = "course-does-not-exist" });
        }
        
        logger.LogInformation($"Course with ID {id} deleted successfully");
        return Ok();
    }
}