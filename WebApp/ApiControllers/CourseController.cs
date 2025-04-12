using System.Security.Claims;
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
    IUserManagementService userManagementService,
    ILogger<CourseController> logger)
    : ControllerBase
{
    
    [Authorize(Roles = "Teacher")]
    [HttpGet("Id/{id}")]
    public async Task<ActionResult<CourseEntity>> GetCourseDetails(int id)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        var tokenUniId = User.FindFirst(ClaimTypes.UserData)?.Value;
        var courseEntity = await courseManagementService.GetCourseByIdAsync(id, tokenUniId!);

        if (courseEntity == null)
        {
            return NotFound(new {message = "Course not found", error = "course-not-found"});
        }
        
        logger.LogInformation($"Successfully fetched course by course ID {id}");
        return Ok(courseEntity);
    }
    
    [Authorize]
    [HttpGet("AttendanceId/{id}")]
    public async Task<ActionResult<CourseEntity>> GetCourseByAttendanceId(int id)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        var courseEntity = await courseManagementService.GetCourseByAttendanceIdAsync(id);

        if (courseEntity == null)
        {
            return NotFound(new {message = "Course not found", error = "course-not-found"});
        }
        
        logger.LogInformation($"Successfully fetched course by attendance with ID {id}");
        return Ok(courseEntity);
    }
    
    [Authorize(Roles = "Teacher")]
    [HttpGet("Statuses")]
    public ActionResult<IEnumerable<CourseStatusDto>> GetAllCourseStatuses()
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        var courseStatuses = courseManagementService.GetAllCourseStatuses();

        if (courseStatuses == null)
        {
            return NotFound(new {message = "Course statuses not found", error = "course-statuses-not-found"});
        }
        
        logger.LogInformation($"All course statuses fetched successfully");
        return Ok(courseStatuses);
    }
    
    [Authorize(Roles = "Teacher")]
    [HttpGet("UniId/{uniId}")]
    public async Task<ActionResult<IEnumerable<CourseEntity>>> GetAllCoursesByUser(string uniId)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        var user = await userManagementService.GetUserByUniIdAsync(uniId);
        if(user == null)
        {
            return NotFound(new {message = "User not found", error = "user-not-found"});
        }
        
        var result = await courseManagementService.GetCoursesByUserAsync(user.Id);
        
        if (result == null)
        {
            return NotFound(new {message = "No courses found", error = "courses-not-found"});
        }
        
        logger.LogInformation($"All courses for user with UNI-ID {uniId}");
        return Ok(result);
    }
    
    [Authorize(Roles = "Teacher")]
    [HttpGet("StudentCounts/{id}")]
    public async Task<ActionResult<IEnumerable<CourseUserCountDto>>> GetAllStudentCountsByCourse(int id)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        var validity = await courseManagementService.DoesCourseExistByIdAsync(id);
        if(!validity)
        {
            return NotFound(new {message = "Course not found", error = "course-not-found"});
        }
        
        var result = await courseManagementService.GetAttendancesUserCountsByCourseAsync(id);

        if (result == null)
        {
            return NotFound(new {message = "No student counts found", error = "student-counts-not-found"});

        }
        
        logger.LogInformation($"All student counts for course with ID {id}");
        return Ok(result);
    }
    
    [Authorize(Roles = "Teacher")]
    [HttpPost("Add")]
    public async Task<ActionResult<CourseEntity>> AddCourse([FromBody] CourseModel model)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        if (!ModelState.IsValid)
        {
            logger.LogWarning($"Form data is invalid");
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
        
        logger.LogInformation($"Course added successfully");
        return Ok();
    }
    
    [Authorize(Roles = "Teacher")]
    [HttpPatch("Edit")]
    public async Task<ActionResult<CourseEntity>> EditCourse([FromBody] CourseModel model)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        if (!ModelState.IsValid || model.Id == null)
        {
            logger.LogWarning($"Form data is invalid");
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

        var courseId = model.Id ?? 0;
        if (!await courseManagementService.EditCourse(courseId, newCourse))
        {
            return BadRequest(new { message = "Course does not exist", error = "course-does-not-exist" });
        }
        
        logger.LogInformation($"Course with ID {model.Id} updated successfully");
        return Ok();
    }
    
    [Authorize(Roles = "Teacher")]
    [HttpDelete("Delete/{id}")]
    public async Task<ActionResult<CourseEntity>> DeleteCourse(int id)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        if (!ModelState.IsValid)
        {
            logger.LogWarning($"Form data is invalid");
            return BadRequest(new { message = "Invalid credentials", error = "invalid-credentials" });
        }
        var tokenUniId = User.FindFirst(ClaimTypes.UserData)?.Value;
        if (!await courseManagementService.DeleteCourse(id, tokenUniId!))
        {
            return BadRequest(new { message = "Course does not exist", error = "course-does-not-exist" });
        }
        
        logger.LogInformation($"Course with ID {id} deleted successfully");
        return Ok();
    }
}