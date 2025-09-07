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
        ILogger<UserController> logger)
        : ControllerBase
    {

        [HttpGet("TestConnection")]
        public ActionResult TestConnection()
        {
            return Ok();
        }
        
        [Authorize(Policy = nameof(EAccessLevel.PrimaryLevel))]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
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
        [HttpGet("UniId/{uniId}")]
        public async Task<ActionResult<UserDto>> GetUserEntityByUniId(string uniId)
        {
            logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
            var userEntity = await userManagementService.GetUserByUniIdAsync(uniId);

            if (userEntity == null)
            {
                return NotFound(new {message = "User not found", messageCode = "user-not-found"});
            }

            var result = new UserDto(userEntity);
            
            logger.LogInformation($"User with UNI-ID {uniId} fetched successfully");
            return Ok(result);
        }
        
        [Authorize(Policy = nameof(EAccessLevel.PrimaryLevel))]
        [HttpGet("Id/{id}")]
        public async Task<ActionResult<UserDto>> GetUserEntity(Guid id)
        {
            logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
            var userEntity = await userManagementService.GetUserByIdAsync(id);

            if (userEntity == null)
            {
                return NotFound(new {message = "User not found", messageCode = "user-not-found"});
            }
            
            var tokenUniId = User.FindFirst(ClaimTypes.UserData)?.Value;
            if (userEntity.UniId != tokenUniId)
            {
                return Unauthorized(new {message = "User not accessible", messageCode = "user-not-accessible"});
            }
            
            var result = new UserDto(userEntity);

            logger.LogInformation($"User with ID {id} fetched successfully");
            return Ok(result);
        }

        [Authorize(Policy = nameof(EAccessLevel.PrimaryLevel))]
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteUserEntity(Guid Id)
        {
            logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
            var userEntity = await userManagementService.GetUserByIdAsync(Id);
            if (userEntity == null)
            {
                return NotFound(new {message = "User not found", messageCode = "user-not-found"});
            }
            
            var tokenUserId = User.FindFirst(ClaimTypes.UserData)?.Value ?? string.Empty;
            if (userEntity.Id != Guid.Parse(tokenUserId))
            {
                return Unauthorized(new {message = "User not accessible", messageCode = "user-not-accessible"});
            }
            
            if (await userManagementService.DeleteUserAsync(userEntity))
            {
                logger.LogInformation($"User with ID {Id} deleted successfully");
                return Ok(new { message = "User deleted successfully" });
            }
            
            return BadRequest(new {message = "User delete failed", messageCode = "user-delete-failed"});
        }
    }
}
