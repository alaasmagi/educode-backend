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
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path} - " +
                              $"{HttpContext.Request.Host.ToString()}");
        var attendanceEntity = await attendanceManagementService.GetCourseAttendanceByIdAsync(id);

        if (attendanceEntity == null)
        {
            logger.LogWarning($"Attendance with id {id} not found");
            return NotFound(new {message = "Attendance not found", error = "attendance-not-found"});
        }
        logger.LogInformation($"Attendance with id {id} found");
        return attendanceEntity;
    }

    [Authorize]
    [HttpGet("CurrentAttendance/UniId/{uniId}")]
    public async Task<ActionResult<CourseAttendanceEntity>> GetCurrenAttendance(string uniId)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path} - " +
                              $"{HttpContext.Request.Host.ToString()}");
        var user = await userManagementService.GetUserByUniIdAsync(uniId);

        if (user == null)
        {
            logger.LogWarning($"User with UNI-ID {uniId} not found");
            return NotFound(new {message = "User not found", error = "user-not-found"});
        }
        
        var courseAttendanceEntity = await attendanceManagementService.GetCurrentAttendanceAsync(user.Id);

        if (courseAttendanceEntity?.Course == null)
        {
            logger.LogWarning($"User with UNI-ID {uniId} has no ongoing attendances");
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
    
    [Authorize(Roles = "Teacher")]
    [HttpGet("CourseCode/{code}")]
    public async Task<ActionResult<CourseAttendanceEntity>> GetAttendancesByCourseCode(string courseCode)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path} - " +
                              $"{HttpContext.Request.Host.ToString()}");
        var course = await courseManagementService.GetCourseByCodeAsync(courseCode);

        if (course == null)
        {
            logger.LogWarning($"Course with code {courseCode} not found");
            return NotFound(new {message = "Course not found", error = "course-not-found"});
        }
        
        var attendances = await attendanceManagementService.GetAttendancesByCourseAsync(course.Id);

        if (attendances == null)
        {
            logger.LogWarning($"Course with code {courseCode} has no attendances");
            return Ok(new {message = "Course has no attendances", error = "no-course-attendances-found"});
        }
        
        return Ok(attendances);
    }
    
    [Authorize(Roles = "Teacher")]
    [HttpGet("CourseName/{courseName}")]
    public async Task<ActionResult<IEnumerable<CourseAttendanceEntity>>> GetAttendancesByCourseName(string courseName)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path} - " +
                              $"{HttpContext.Request.Host.ToString()}");
        var course = await courseManagementService.GetCourseByNameAsync(courseName);

        if (course == null)
        {
            return NotFound(new {message = "Course not found", error = "course-not-found"});
        }
        
        var attendances = await attendanceManagementService.GetAttendancesByCourseAsync(course.Id);

        if (attendances == null)
        {
            return Ok(new {message = "Course has no attendances", error = "no-course-attendances-found"});
        }
        
        return Ok(attendances);
    }
    
    [Authorize(Roles = "Teacher")]
    [HttpGet("RecentAttendance/UniId/{uniId}")]
    public async Task<ActionResult<CourseAttendanceEntity>> GetMostRecentAttendance(string uniId)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path} - " +
                              $"{HttpContext.Request.Host.ToString()}");
        var user= await userManagementService.GetUserByUniIdAsync(uniId);

        if (user == null)
        {
            return NotFound(new {message = "User not found", error = "user-not-found"});
        }
        
        var attendance = await attendanceManagementService.GetMostRecentAttendanceByUserAsync(user.Id);

        if (attendance == null)
        {
            return Ok(new {message = "Course has no attendances", error = "no-course-attendances-found"});
        }
        
        return Ok(attendance);
    }
    
    [Authorize(Roles = "Teacher")]
    [HttpGet("AttendanceChecks/AttendanceId/{attendanceId}")]
    public async Task<ActionResult<IEnumerable<AttendanceCheckEntity>>> GetAttendanceChecksByAttendanceId(int attendanceId)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path} - " +
                              $"{HttpContext.Request.Host.ToString()}");
        var user = await attendanceManagementService.GetCourseAttendanceByIdAsync(attendanceId);

        if (user == null)
        {
            return NotFound(new {message = "Attendance not found", error = "attendance-not-found"});
        }
        
        var attendanceChecks = await attendanceManagementService.GetAttendanceChecksByAttendanceIdAsync(attendanceId);
        if (attendanceChecks == null)
        {
            return Ok(new {message = "Attendance has no attendance checks", error = "attendance-has-no-checks"});
        }
        
        return Ok(attendanceChecks);
    }
    
    [Authorize(Roles = "Teacher")]
    [HttpGet("AttendanceTypes")]
    public async Task<ActionResult<IEnumerable<AttendanceTypeEntity>>> GetAllAttendanceTypes()
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path} - " +
                              $"{HttpContext.Request.Host.ToString()}");
        var attendanceTypes = await attendanceManagementService.GetAttendanceTypesAsync();
        
        return Ok(attendanceTypes);
    }
    
    [Authorize]
    [HttpPost("AttendanceCheck/Add")]
    public async Task<IActionResult> AddAttendanceCheck([FromBody] AttendanceCheckModel model)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path} - " +
                              $"{HttpContext.Request.Host.ToString()}");
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
    
    [Authorize(Roles = "Teacher")]
    [HttpPost("Add")]
    public async Task<ActionResult<CourseAttendanceEntity>> AddCourseAttendance([FromBody] AttendanceModel model)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path} - " +
                              $"{HttpContext.Request.Host.ToString()}");
        if (!ModelState.IsValid)
        {
            return BadRequest(new { message = "Invalid credentials", error = "invalid-credentials" });
        }
        
        var course = await courseManagementService.GetCourseByIdAsync(model.CourseId);
        if (course == null)
        {
            return NotFound(new {message = "Course not found", error = "course-not-found"});
        }
        
        var attendanceType = await attendanceManagementService.GetAttendanceTypeByIdAsync(model.AttendanceTypeId);
        if (attendanceType == null)
        {
            return NotFound(new {message = "Attendance type not found", error = "attendance-type-not-found"});
        }
        
        var newAttendance = new CourseAttendanceEntity()
        {
            CourseId = model.CourseId,
            Course = course,
            AttendanceTypeId = model.AttendanceTypeId,
            AttendanceType = attendanceType,
            CreatedBy = model.Creator,
            UpdatedBy = model.Creator
        };
        await attendanceManagementService.AddAttendanceAsync(newAttendance, model.AttendanceDates, model.StartTime, 
                                                                                                model.EndTime);
        return Ok();
    }
    
    [Authorize(Roles = "Teacher")]
    [HttpPatch("Edit")]
    public async Task<ActionResult<CourseEntity>> EditAttendance([FromBody] AttendanceModel model)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path} - " +
                               $"{HttpContext.Request.Host.ToString()}");
        if (!ModelState.IsValid || model.Id == null)
        {
            return BadRequest(new { message = "Invalid credentials", error = "invalid-credentials" });
        }
        
        var course = await courseManagementService.GetCourseByIdAsync(model.CourseId);
        if (course == null)
        {
            return NotFound(new {message = "Course not found", error = "course-not-found"});
        }
        
        var attendanceType = await attendanceManagementService.GetAttendanceTypeByIdAsync(model.AttendanceTypeId);
        if (attendanceType == null)
        {
            return NotFound(new {message = "Attendance type not found", error = "attendance-type-not-found"});
        }
        
        var newAttendance = new CourseAttendanceEntity()
        {
            CourseId = model.CourseId,
            Course = course,
            AttendanceTypeId = model.AttendanceTypeId,
            AttendanceType = attendanceType,
            StartTime = model.AttendanceDates[0].ToDateTime(model.StartTime).ToUniversalTime(),
            EndTime = model.AttendanceDates[0].ToDateTime(model.EndTime).ToUniversalTime(),
            CreatedBy = model.Creator,
            UpdatedBy = model.Creator
        };

        var attendanceId = model.Id ?? 0;
        if (!await attendanceManagementService.EditAttendanceAsync(attendanceId, newAttendance))
        {
            return BadRequest(new { message = "Attendance does not exist", error = "attendance-does-not-exist" });
        }

        return Ok();
    }
    
    [Authorize(Roles = "Teacher")]
    [HttpDelete("Delete/{id}")]
    public async Task<ActionResult<CourseEntity>> DeleteAttendance(int id)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path} - " +
                              $"{HttpContext.Request.Host.ToString()}");
        if (!ModelState.IsValid)
        {
            return BadRequest(new { message = "Invalid credentials", error = "invalid-credentials" });
        }
        
        if (!await attendanceManagementService.DeleteAttendance(id))
        {
            return BadRequest(new { message = "Attendance does not exist", error = "attendance-does-not-exist" });
        }

        return Ok();
    }
    
    [Authorize(Roles = "Teacher")]
    [HttpDelete("Delete/AttendanceCheck/{id}")]
    public async Task<ActionResult<CourseEntity>> DeleteAttendanceCheck(int id)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path} - " +
                              $"{HttpContext.Request.Host.ToString()}");
        if (!ModelState.IsValid)
        {
            return BadRequest(new { message = "Invalid credentials", error = "invalid-credentials" });
        }

        if (!await attendanceManagementService.DeleteAttendanceCheck(id))
        {
            return BadRequest(new { message = "AttendanceCheck does not exist", 
                                                                        error = "attendance-check-does-not-exist" });
        }
        
        return Ok();
    }
}