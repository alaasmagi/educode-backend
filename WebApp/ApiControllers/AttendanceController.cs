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
public class AttendanceController(
    IAttendanceManagementService attendanceManagementService,
    ICourseManagementService courseManagementService,
    IUserManagementService userManagementService,
    ILogger<AttendanceController> logger)
    : ControllerBase
{
    [Authorize(Policy = nameof(EAccessLevel.SecondaryLevel))]
    [HttpGet("Id/{id}")]
    public async Task<ActionResult<CourseAttendanceDto>> GetAttendanceById(Guid id)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
            
        var tokenUserId = User.FindFirst(ClaimTypes.UserData)?.Value ?? string.Empty;
        var attendanceEntity = await attendanceManagementService.GetCourseAttendanceByIdAsync(id, tokenUserId);

        if (attendanceEntity == null)
        {
            return NotFound(new {message = "Attendance not found", messageCode = "attendance-not-found"});
        }

        var result = new CourseAttendanceDto(attendanceEntity);
        
        logger.LogInformation($"Attendance with ID {id} successfully fetched");
        return result;
    }

    [Authorize(Policy = nameof(EAccessLevel.PrimaryLevel))]
    [HttpGet("CurrentAttendance")]
    public async Task<ActionResult<CourseAttendanceDto>> GetCurrenAttendance()
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        var tokenUserId = User.FindFirst(ClaimTypes.UserData)?.Value ?? string.Empty;
        var user = await userManagementService.GetUserByIdAsync(Guid.Parse(tokenUserId));

        if (user == null)
        {
            return NotFound(new {message = "User not found", messageCode = "user-not-found"});
        }
        
        var courseAttendanceEntity = await attendanceManagementService.GetCurrentAttendanceAsync(user.Id);

        if (courseAttendanceEntity?.Course == null)
        {
            return Ok(new {message = "Current attendance not found", messageCode = "current-attendance-not-found"});
        }
        
        var result = new CourseAttendanceDto(courseAttendanceEntity);
        
        logger.LogInformation($"Current attendance for ID {tokenUserId} successfully fetched");
        return Ok(result);
    }
    
    [Authorize(Policy = nameof(EAccessLevel.TertiaryLevel))]
    [HttpGet("StudentCount/AttendanceId/{id}")]
    public async Task<ActionResult<int>> GetAttendanceStudentCount(Guid id)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        
        var tokenUserId = User.FindFirst(ClaimTypes.UserData)?.Value ?? string.Empty;
        var attendance = await attendanceManagementService.GetCourseAttendanceByIdAsync(id, tokenUserId);

        if (attendance == null)
        {
            return NotFound(new {message = "Attendance not found", messageCode = "attendance-not-found"});
        }
        
        var studentCount = await attendanceManagementService.GetStudentsCountByAttendanceIdAsync(attendance.Identifier);
       
        logger.LogInformation($"Students count for attendance with ID {id} successfully fetched");
        return Ok(studentCount);
    }
    
    
    [Authorize(Policy = nameof(EAccessLevel.TertiaryLevel))]
    [HttpGet("CourseCode/{courseCode}")]
    public async Task<ActionResult<IEnumerable<CourseAttendanceDto>>> GetAttendancesByCourseCode(string courseCode)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        
        var tokenUserId = User.FindFirst(ClaimTypes.UserData)?.Value ?? string.Empty;
        var course = await courseManagementService.GetCourseByCodeAsync(courseCode, tokenUserId);

        if (course == null)
        {
            return NotFound(new {message = "Course not found", messageCode = "course-not-found"});
        }
        
        var attendances = 
            await attendanceManagementService.GetAttendancesByCourseAsync(course.Id);

        if (attendances == null)
        {
            return Ok(new {message = "Course has no attendances", messageCode = "no-course-attendances-found"});
        }
        
        var result = CourseAttendanceDto.ToDtoList(attendances);
        
        logger.LogInformation($"Attendances for course {courseCode} successfully fetched");
        return Ok(result);
    }
    
    [Authorize(Policy = nameof(EAccessLevel.TertiaryLevel))]
    [HttpGet("CourseName/{courseName}")]
    public async Task<ActionResult<IEnumerable<CourseAttendanceDto>>> GetAttendancesByCourseName(string courseName)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        var tokenUserId = User.FindFirst(ClaimTypes.UserData)?.Value ?? string.Empty;

        var course = await courseManagementService.GetCourseByNameAsync(courseName, tokenUserId);

        if (course == null)
        {
            return NotFound(new {message = "Course not found", messageCode = "course-not-found"});
        }
        
        var attendances = 
            await attendanceManagementService.GetAttendancesByCourseAsync(course.Id);

        if (attendances == null)
        {
            return Ok(new {message = "Course has no attendances", messageCode = "no-course-attendances-found"});
        }
        
        var result = CourseAttendanceDto.ToDtoList(attendances);
        
        logger.LogInformation($"Attendances for course {courseName} successfully fetched");
        return Ok(result);
    }
    
    [Authorize(Policy = nameof(EAccessLevel.TertiaryLevel))]
    [HttpGet("RecentAttendance")]
    public async Task<ActionResult<CourseAttendanceDto>> GetMostRecentAttendance()
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        var tokenUserId = User.FindFirst(ClaimTypes.UserData)?.Value ?? string.Empty;
        var user= await userManagementService.GetUserByIdAsync(Guid.Parse(tokenUserId));

        if (user == null)
        {
            return NotFound(new {message = "User not found", messageCode = "user-not-found"});
        }
        
        var attendance = await attendanceManagementService.GetMostRecentAttendanceByUserAsync(user.Id);

        if (attendance == null)
        {
            return Ok(new {message = "User has no recent attendances", messageCode = "no-user-recent-attendances-found"});
        }

        var result = new CourseAttendanceDto(attendance);
        
        logger.LogInformation($"Most recent attendance for user with ID {tokenUserId} successfully fetched");
        return Ok(result);
    }
    
    [Authorize(Policy = nameof(EAccessLevel.TertiaryLevel))]
    [HttpGet("AttendanceChecks/AttendanceId/{attendanceId}")]
    public async Task<ActionResult<IEnumerable<AttendanceCheckDto>>> GetAttendanceChecksByAttendanceId(Guid attendanceId)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        var tokenUserId = User.FindFirst(ClaimTypes.UserData)?.Value ?? string.Empty;
        var courseAttendance = await attendanceManagementService.GetCourseAttendanceByIdAsync(attendanceId, tokenUserId);

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
        
        logger.LogInformation($"Attendance checks for attendance with ID {attendanceId} successfully fetched");
        return Ok(result);
    }
    
    [Authorize(Policy = nameof(EAccessLevel.TertiaryLevel))]
    [HttpGet("AttendanceTypes")]
    public async Task<ActionResult<IEnumerable<AttendanceTypeDto>>> GetAllAttendanceTypes()
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        var attendanceTypes = await attendanceManagementService.GetAttendanceTypesAsync();

        if (attendanceTypes == null)
        {
            return NotFound(new {message = "Attendance types not found", messageCode = "attendance-types-not-found"});
        }
        
        var result = AttendanceTypeDto.ToDtoList(attendanceTypes);
        
        logger.LogInformation($"All attendance types successfully fetched");
        return Ok(result);
    }
    
    [Authorize(Policy = nameof(EAccessLevel.TertiaryLevel))]
    [HttpPost("AttendanceCheck/Add")]
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
    [HttpPost("Add")]
    public async Task<ActionResult> AddCourseAttendance([FromBody] AttendanceModel model)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        if (!ModelState.IsValid)
        {
            return BadRequest(new { message = "Invalid credentials", messageCode = "invalid-credentials" });
        }
        var tokenUserId = User.FindFirst(ClaimTypes.UserData)?.Value ?? string.Empty;
        var course = await courseManagementService.GetCourseByIdAsync(model.CourseId, tokenUserId);
        if (course == null)
        {
            return NotFound(new {message = "Course not found", messageCode = "course-not-found"});
        }
        
        var attendanceType = await attendanceManagementService.GetAttendanceTypeByIdAsync(model.AttendanceTypeId);
        if (attendanceType == null)
        {
            return NotFound(new {message = "Attendance type not found", messageCode = "attendance-type-not-found"});
        }
        
        var newAttendance = new CourseAttendanceEntity()
        {
            CourseId = model.CourseId,
            AttendanceTypeId = model.AttendanceTypeId,
            CreatedBy = model.Creator,
            UpdatedBy = model.Creator
        };
        if (!await attendanceManagementService.AddAttendanceAsync(newAttendance, model.AttendanceDates, model.StartTime,
                model.EndTime))
        {
            return BadRequest(new {message = "One or more attendances could not be added", 
                messageCode = "attendances-could-not-be-added"});
        }
        
        logger.LogInformation($"Attendance added successfully");
        return Ok();
    }
    
    [Authorize(Policy = nameof(EAccessLevel.TertiaryLevel))]
    [HttpPatch("Edit")]
    public async Task<ActionResult> EditAttendance([FromBody] AttendanceModel model)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        if (!ModelState.IsValid || model.Id == null)
        {
            logger.LogWarning($"Form data is invalid");
            return BadRequest(new { message = "Invalid credentials", messageCode = "invalid-credentials" });
        }
        
        var tokenUserId = User.FindFirst(ClaimTypes.UserData)?.Value ?? string.Empty;
        var course = await courseManagementService.GetCourseByIdAsync(model.CourseId, tokenUserId);
        if (course == null)
        {
            return NotFound(new {message = "Course not found", messageCode = "course-not-found"});
        }
        
        var attendanceType = await attendanceManagementService.GetAttendanceTypeByIdAsync(model.AttendanceTypeId);
        if (attendanceType == null)
        {
            return NotFound(new {message = "Attendance type not found", messageCode = "attendance-type-not-found"});
        }
        
        var newAttendance = new CourseAttendanceEntity()
        {
            CourseId = model.CourseId,
            AttendanceTypeId = model.AttendanceTypeId,
            StartTime = model.AttendanceDates[0].ToDateTime(model.StartTime).ToUniversalTime(),
            EndTime = model.AttendanceDates[0].ToDateTime(model.EndTime).ToUniversalTime(),
            CreatedBy = model.Creator,
            UpdatedBy = model.Creator
        };

        var attendanceId = model.Id.Value;
        if (!await attendanceManagementService.EditAttendanceAsync(attendanceId, newAttendance))
        {
            return BadRequest(new { message = "Attendance does not exist", messageCode = "attendance-does-not-exist" });
        }

        logger.LogInformation($"Attendance for attendance with ID {model.Id} updated successfully");
        return Ok();
    }
    
    [Authorize(Policy = nameof(EAccessLevel.TertiaryLevel))]
    [HttpDelete("Delete/{id}")]
    public async Task<ActionResult> DeleteAttendance(Guid id)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        if (!ModelState.IsValid)
        {
            logger.LogWarning($"Form data is invalid");
            return BadRequest(new { message = "Invalid credentials", messageCode = "invalid-credentials" });
        }
        
        var tokenUserId = User.FindFirst(ClaimTypes.UserData)?.Value ?? string.Empty;
        if (!await attendanceManagementService.DeleteAttendance(id, tokenUserId))
        {
            return BadRequest(new { message = "Attendance does not exist", messageCode = "attendance-does-not-exist" });
        }

        logger.LogInformation($"Attendance with ID {id} deleted successfully");
        return Ok();
    }
    
    [Authorize(Policy = nameof(EAccessLevel.TertiaryLevel))]
    [HttpDelete("AttendanceCheck/Delete/{id}")]
    public async Task<ActionResult> DeleteAttendanceCheck(Guid id)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        if (!ModelState.IsValid)
        {
            logger.LogWarning($"Form data is invalid");
            return BadRequest(new { message = "Invalid credentials", messageCode = "invalid-credentials" });
        }

        var tokenUserId = User.FindFirst(ClaimTypes.UserData)?.Value ?? string.Empty;
        if (!await attendanceManagementService.DeleteAttendanceCheck(id, tokenUserId))
        {
            return BadRequest(new { message = "AttendanceCheck does not exist", 
                messageCode = "attendance-check-does-not-exist" });
        }
        
        logger.LogInformation($"Attendance check with ID {id} deleted successfully");
        return Ok();
    }
}