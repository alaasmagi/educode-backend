﻿using System.Security.Claims;
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
    ICourseManagementService courseManagementService,
    IUserManagementService userManagementService,
    ILogger<AttendanceController> logger)
    : ControllerBase
{
    [Authorize]
    [HttpGet("Id/{id}")]
    public async Task<ActionResult<CourseAttendanceEntity>> GetAttendanceById(int id)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
            
        var tokenUniId = User.FindFirst(ClaimTypes.UserData)?.Value ?? string.Empty;
        var attendanceEntity = await attendanceManagementService.GetCourseAttendanceByIdAsync(id, tokenUniId);

        if (attendanceEntity == null)
        {
            return NotFound(new {message = "Attendance not found", messageCode = "attendance-not-found"});
        }
        
        logger.LogInformation($"Attendance with ID {id} successfully fetched");
        return attendanceEntity;
    }

    [Authorize]
    [HttpGet("CurrentAttendance/UniId/{uniId}")]
    public async Task<ActionResult<CourseAttendanceEntity>> GetCurrenAttendance(string uniId)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        var user = await userManagementService.GetUserByUniIdAsync(uniId);

        if (user == null)
        {
            return NotFound(new {message = "User not found", messageCode = "user-not-found"});
        }
        
        var courseAttendanceEntity = await attendanceManagementService.GetCurrentAttendanceAsync(user.Id);

        if (courseAttendanceEntity?.Course == null)
        {
            return Ok(new {message = "Current attendance not found", messageCode = "current-attendance-not-found"});
        }
        
        logger.LogInformation($"Current attendance for UNI-ID {uniId} successfully fetched");
        return Ok(courseAttendanceEntity);
    }
    
    [Authorize(Roles="Teacher")]
    [HttpGet("StudentCount/AttendanceId/{id}")]
    public async Task<ActionResult<int>> GetAttendanceStudentCount(int id)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        
        var tokenUniId = User.FindFirst(ClaimTypes.UserData)?.Value;
        var attendance = await attendanceManagementService.GetCourseAttendanceByIdAsync(id, tokenUniId!);

        if (attendance == null)
        {
            return NotFound(new {message = "Attendance not found", messageCode = "attendance-not-found"});
        }
        
        var studentCount = await attendanceManagementService.GetStudentsCountByAttendanceIdAsync(id);
       
        logger.LogInformation($"Students count for attendance with ID {id} successfully fetched");
        return Ok(studentCount);
    }
    
    
    [Authorize(Roles = "Teacher")]
    [HttpGet("CourseCode/{courseCode}")]
    public async Task<ActionResult<CourseAttendanceEntity>> GetAttendancesByCourseCode(string courseCode)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        
        var tokenUniId = User.FindFirst(ClaimTypes.UserData)?.Value;
        var course = await courseManagementService.GetCourseByCodeAsync(courseCode, tokenUniId!);

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
        
        logger.LogInformation($"Attendances for course {courseCode} successfully fetched");
        return Ok(attendances);
    }
    
    [Authorize(Roles = "Teacher")]
    [HttpGet("CourseName/{courseName}")]
    public async Task<ActionResult<IEnumerable<CourseAttendanceEntity>>> GetAttendancesByCourseName(string courseName)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        var tokenUniId = User.FindFirst(ClaimTypes.UserData)?.Value;

        var course = await courseManagementService.GetCourseByNameAsync(courseName, tokenUniId!);

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
        
        logger.LogInformation($"Attendances for course {courseName} successfully fetched");
        return Ok(attendances);
    }
    
    [Authorize(Roles = "Teacher")]
    [HttpGet("RecentAttendance/UniId/{uniId}")]
    public async Task<ActionResult<CourseAttendanceEntity>> GetMostRecentAttendance(string uniId)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        var user= await userManagementService.GetUserByUniIdAsync(uniId);

        if (user == null)
        {
            return NotFound(new {message = "User not found", messageCode = "user-not-found"});
        }
        
        var attendance = await attendanceManagementService.GetMostRecentAttendanceByUserAsync(user.Id);

        if (attendance == null)
        {
            return Ok(new {message = "User has no recent attendances", messageCode = "no-user-recent-attendances-found"});
        }
        
        logger.LogInformation($"Most recent attendance for user with UNI-ID {uniId} successfully fetched");
        return Ok(attendance);
    }
    
    [Authorize(Roles = "Teacher")]
    [HttpGet("AttendanceChecks/AttendanceId/{attendanceId}")]
    public async Task<ActionResult<IEnumerable<AttendanceCheckEntity>>> GetAttendanceChecksByAttendanceId(int attendanceId)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        var tokenUniId = User.FindFirst(ClaimTypes.UserData)?.Value;
        var courseAttendance = await attendanceManagementService.GetCourseAttendanceByIdAsync(attendanceId, tokenUniId!);

        if (courseAttendance == null)
        {
            return NotFound(new {message = "Attendance not found", messageCode = "attendance-not-found"});
        }
        
        var attendanceChecks = 
            await attendanceManagementService.GetAttendanceChecksByAttendanceIdAsync(attendanceId);
        if (attendanceChecks == null)
        {
            return Ok(new {message = "Attendance has no attendance checks", messageCode = "attendance-has-no-checks"});
        }
        
        logger.LogInformation($"Attendance checks for attendance with ID {attendanceId} successfully fetched");
        return Ok(attendanceChecks);
    }
    
    [Authorize(Roles = "Teacher")]
    [HttpGet("AttendanceTypes")]
    public async Task<ActionResult<IEnumerable<AttendanceTypeEntity>>> GetAllAttendanceTypes()
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        var attendanceTypes = await attendanceManagementService.GetAttendanceTypesAsync();

        if (attendanceTypes == null)
        {
            return NotFound(new {message = "Attendance types not found", messageCode = "attendance-types-not-found"});
        }
        
        logger.LogInformation($"All attendance types successfully fetched");
        return Ok(attendanceTypes);
    }
    
    [Authorize]
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
            CourseAttendanceId = model.CourseAttendanceId,
            CreatedBy = model.Creator,
            UpdatedBy = model.Creator,
        };

        if (model.WorkplaceId != null)
        {
            int workplaceId = model.WorkplaceId.Value;
            if(!await attendanceManagementService.DoesWorkplaceExist(workplaceId))
            {
                return NotFound(new {message = "Workplace was not found ", messageCode = "workplace-not-found"});
            }
        }

        if (!await attendanceManagementService.AddAttendanceCheckAsync(newAttendanceCheck, model.Creator, model.WorkplaceId ?? null))
        {
            return BadRequest(new {message = "Attendance check already exists", 
                messageCode = "attendance-check-already-exists" });
        }

        logger.LogInformation($"Attendance check added successfully");
        return Ok();
    }
    
    [Authorize(Roles = "Teacher")]
    [HttpPost("Add")]
    public async Task<ActionResult<CourseAttendanceEntity>> AddCourseAttendance([FromBody] AttendanceModel model)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        if (!ModelState.IsValid)
        {
            return BadRequest(new { message = "Invalid credentials", messageCode = "invalid-credentials" });
        }
        var tokenUniId = User.FindFirst(ClaimTypes.UserData)?.Value;
        var course = await courseManagementService.GetCourseByIdAsync(model.CourseId, tokenUniId!);
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
    
    [Authorize(Roles = "Teacher")]
    [HttpPatch("Edit")]
    public async Task<ActionResult<CourseEntity>> EditAttendance([FromBody] AttendanceModel model)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        if (!ModelState.IsValid || model.Id == null)
        {
            logger.LogWarning($"Form data is invalid");
            return BadRequest(new { message = "Invalid credentials", messageCode = "invalid-credentials" });
        }
        
        var tokenUniId = User.FindFirst(ClaimTypes.UserData)?.Value;
        var course = await courseManagementService.GetCourseByIdAsync(model.CourseId, tokenUniId!);
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

        var attendanceId = model.Id ?? 0;
        if (!await attendanceManagementService.EditAttendanceAsync(attendanceId, newAttendance))
        {
            return BadRequest(new { message = "Attendance does not exist", messageCode = "attendance-does-not-exist" });
        }

        logger.LogInformation($"Attendance for attendance with ID {model.Id} updated successfully");
        return Ok();
    }
    
    [Authorize(Roles = "Teacher")]
    [HttpDelete("Delete/{id}")]
    public async Task<ActionResult<CourseEntity>> DeleteAttendance(int id)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        if (!ModelState.IsValid)
        {
            logger.LogWarning($"Form data is invalid");
            return BadRequest(new { message = "Invalid credentials", messageCode = "invalid-credentials" });
        }
        
        var tokenUniId = User.FindFirst(ClaimTypes.UserData)?.Value;
        if (!await attendanceManagementService.DeleteAttendance(id, tokenUniId!))
        {
            return BadRequest(new { message = "Attendance does not exist", messageCode = "attendance-does-not-exist" });
        }

        logger.LogInformation($"Attendance with ID {id} deleted successfully");
        return Ok();
    }
    
    [Authorize(Roles = "Teacher")]
    [HttpDelete("AttendanceCheck/Delete/{id}")]
    public async Task<ActionResult<CourseEntity>> DeleteAttendanceCheck(int id)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        if (!ModelState.IsValid)
        {
            logger.LogWarning($"Form data is invalid");
            return BadRequest(new { message = "Invalid credentials", messageCode = "invalid-credentials" });
        }

        var tokenUniId = User.FindFirst(ClaimTypes.UserData)?.Value;
        if (!await attendanceManagementService.DeleteAttendanceCheck(id, tokenUniId!))
        {
            return BadRequest(new { message = "AttendanceCheck does not exist", 
                messageCode = "attendance-check-does-not-exist" });
        }
        
        logger.LogInformation($"Attendance check with ID {id} deleted successfully");
        return Ok();
    }
}