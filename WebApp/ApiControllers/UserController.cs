using System.Security.Claims;
using App.BLL;
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
        IPhotoService photoService,
        EnvInitializer envInitializer,
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
           
           var result = UserDto.ToDtoList(users, envInitializer.BucketUrl);
           
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
            
            var result = new UserDto(userEntity, envInitializer.BucketUrl);

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
        
        [Authorize(Policy = nameof(EAccessLevel.PrimaryLevel))]
        [HttpPost("{id}/UploadPhoto")]
        public async Task<ActionResult> UploadUserPhoto(Guid id)
        {
            logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
            var userId = User.FindFirst(Constants.UserIdClaim)?.Value ?? string.Empty;
            var user = await userManagementService.GetUserByIdAsync(Guid.Parse(userId));

            if (user == null)
            {
                return NotFound(new {message = "User not found", messageCode = "user-not-found"});
            }
            
            var file = HttpContext.Request.Form.Files.FirstOrDefault();

            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "No file uploaded", messageCode = "no-file-uploaded" });
            }
            
            if (!file.ContentType.StartsWith("image/"))
            {
                return BadRequest(new { message = "Invalid file type. Only images allowed.", messageCode = "invalid-file-type" });
            }
            
            const int maxFileSizeInBytes = 5 * 1024 * 1024; 
            if (file.Length > maxFileSizeInBytes)
            {
                return BadRequest(new { message = "File size exceeds 5MB limit.", messageCode = "file-too-large" });
            }
            
            using (var photoStream = file.OpenReadStream())
            {
                var objectPath = await photoService.UploadPhotoAsync(
                    Constants.UserFolder,
                    id,
                    photoStream,
                    file.ContentType);

                if (objectPath == null)
                {
                    return StatusCode(500, new { message = "Photo upload failed due to server error.", messageCode = "oci-upload-failed" });
                }
                logger.LogInformation($"Successfully uploaded photo for user {userId}. Path: {objectPath}");
                return Ok(new { message = "Photo uploaded successfully", path = objectPath });
            }
        }
        
        [Authorize(Policy = nameof(EAccessLevel.TertiaryLevel))]
        [HttpDelete("{id}/RemovePhoto")]
        public async Task<ActionResult<CourseAttendanceDto>> RemoveUserPhoto(Guid id)
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
