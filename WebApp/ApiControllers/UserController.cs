using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using App.Domain;
using App.DTO;
using Contracts;

namespace WebApp.ApiControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(
        IUserManagementService userManagementService,
        ICourseManagementService courseManagementService,
        IAttendanceManagementService attendanceManagementService,
        ILogger<UserController> logger)
        : ControllerBase
    {
        [Authorize(Policy = nameof(EAccessLevel.PrimaryLevel))]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 25)
        {
           logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
           var users = await userManagementService.GetAllUsersAsync();
           
           if (users == null)
           {
               return NotFound(new {message = "Users not found", messageCode = "users-not-found"});
           }
           
           var result = UserDto.ToDtoList(users);
           
           logger.LogInformation($"All users fetched successfully");
           return Ok(result);
        }
        
        [Authorize(Policy = nameof(EAccessLevel.PrimaryLevel))]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserEntity(Guid id)
        {
            logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
            var userEntity = await userManagementService.GetUserByIdAsync(id);
            var userId = User.FindFirst(Constants.UserIdClaim)?.Value ?? string.Empty;

            if (userEntity == null)
            {
                return NotFound(new {message = "User not found", messageCode = "user-not-found"});
            }
            
            if (userEntity.Id != Guid.Parse(userId))
            {
                return Unauthorized(new {message = "User not accessible", messageCode = "user-not-accessible"});
            }
            
            var result = new UserDto(userEntity);

            logger.LogInformation($"User with ID {id} fetched successfully");
            return Ok(result);
        }

        [Authorize(Policy = nameof(EAccessLevel.PrimaryLevel))]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateUserEntity(Guid id)
        {
            logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
            var userEntity = await userManagementService.GetUserByIdAsync(id);
            var userId = User.FindFirst(Constants.UserIdClaim)?.Value ?? string.Empty;

            if (userEntity == null)
            {
                return NotFound(new {message = "User not found", messageCode = "user-not-found"});
            }
            
            if (userEntity.Id != Guid.Parse(userId))
            {
                return Unauthorized(new {message = "User not accessible", messageCode = "user-not-accessible"});
            }
            
            if (await userManagementService.DeleteUserAsync(userEntity))
            {
                logger.LogInformation($"User with ID {id} deleted successfully");
                return Ok(new { message = "User deleted successfully" });
            }
            
            return BadRequest(new {message = "User delete failed", messageCode = "user-delete-failed"});
        }
        
        [Authorize(Policy = nameof(EAccessLevel.PrimaryLevel))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserEntity(Guid id)
        {
            logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
            var userEntity = await userManagementService.GetUserByIdAsync(id);
            var userId = User.FindFirst(Constants.UserIdClaim)?.Value ?? string.Empty;

            if (userEntity == null)
            {
                return NotFound(new {message = "User not found", messageCode = "user-not-found"});
            }
            
            if (userEntity.Id != Guid.Parse(userId))
            {
                return Unauthorized(new {message = "User not accessible", messageCode = "user-not-accessible"});
            }
            
            if (await userManagementService.DeleteUserAsync(userEntity))
            {
                logger.LogInformation($"User with ID {id} deleted successfully");
                return Ok(new { message = "User deleted successfully" });
            }
            
            return BadRequest(new {message = "User delete failed", messageCode = "user-delete-failed"});
        }
        
        [Authorize(Policy = nameof(EAccessLevel.TertiaryLevel))]
        [HttpGet("{id}/Courses")]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetAllCoursesByUser(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 25)
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
        
        [Authorize(Policy = nameof(EAccessLevel.PrimaryLevel))]
    
        [HttpGet("{id}/CurrentAttendance")]
        public async Task<ActionResult<CourseAttendanceDto>> GetCurrenAttendance()
        {
            logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
            var userId = User.FindFirst(Constants.UserIdClaim)?.Value ?? string.Empty;
            var user = await userManagementService.GetUserByIdAsync(Guid.Parse(userId));

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
            
            logger.LogInformation($"Current attendance for user with ID {userId} successfully fetched");
            return Ok(result);
        }
        
        [Authorize(Policy = nameof(EAccessLevel.TertiaryLevel))]
        [HttpGet("{id}/RecentAttendance")]
        public async Task<ActionResult<CourseAttendanceDto>> GetMostRecentAttendance()
        {
            logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
            var userId = User.FindFirst(Constants.UserIdClaim)?.Value ?? string.Empty;
            var user= await userManagementService.GetUserByIdAsync(Guid.Parse(userId));

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
            
            logger.LogInformation($"Most recent attendance for user with ID {userId} successfully fetched");
            return Ok(result);
        }
    }
}
