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
    [HttpGet("CurrentAttendance/UniId/{uniId}")]
    public async Task<ActionResult<CourseAttendanceEntity>> GetCurrenAttendance(string uniId)
    {
        var user = await userManagementService.GetUserByUniIdAsync(uniId);

        if (user == null)
        {
            return NotFound(new {message = "User not found", error = "user-not-found"});
        }
        
        var courseAttendanceEntity = await attendanceManagementService.GetCurrentAttendanceAsync(user.Id);

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
    
    [Authorize(Roles = "Teacher")]
    [HttpGet("CourseCode/{code}")]
    public async Task<ActionResult<CourseAttendanceEntity>> GetAttendancesByCourseCode(string courseCode)
    {
        var course = await courseManagementService.GetCourseByCodeAsync(courseCode);

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
    [HttpGet("CourseName/{courseName}")]
    public async Task<ActionResult<IEnumerable<CourseAttendanceEntity>>> GetAttendancesByCourseName(string courseName)
    {
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
    [HttpPost("Add")]
    public async Task<IActionResult> AddAttendance([FromBody] AttendanceCheckModel model)
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