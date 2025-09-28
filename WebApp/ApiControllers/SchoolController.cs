using App.BLL;
using App.Domain;
using App.DTO;
using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.ApiControllers;

[ApiController]
[Route("api/[controller]")]
public class SchoolController(
    EnvInitializer envInitializer,
    ISchoolManagementService schoolManagementService,
    IPhotoService photoService,
    IUserManagementService userManagementService,
    ILogger<OtpController> logger)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<SchoolDto>>> GetAllSchools()
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        
        var schools = await schoolManagementService.GetAllSchools(); 
        if (schools == null)
        {
            return NotFound(new { message = "Schools not found", messageCode = "schools-not-found" });
        }

        var result = SchoolDto.ToDtoList(schools, envInitializer.BucketUrl);

        logger.LogInformation($"{schools.Count} schools successfully fetched");
        return result;
    }
    
    [Authorize(Policy = nameof(EAccessLevel.PrimaryLevel))]
    [HttpPost("{id}/UploadPhoto")]
    public async Task<ActionResult> UploadUserPhoto(Guid id)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        var school = await schoolmanagementService.GetUserByIdAsync(Guid.Parse(userId));

        if (school == null)
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
    public async Task<ActionResult<CourseAttendanceDto>> RemoveSchoolPhoto(Guid id)
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