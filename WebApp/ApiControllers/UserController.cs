using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using App.Domain;
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
        
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserEntity>>> GetUsers()
        {
           logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
           var users = await userManagementService.GetAllUsersAsync();
           
           if (users == null)
           {
               return NotFound(new {message = "Users not found", messageCode = "users-not-found"});
           }
           
           logger.LogInformation($"All users fetched successfully");
           return Ok(users);
        }
        
        [Authorize]
        [HttpGet("UniId/{uniId}")]
        public async Task<IActionResult> GetUserEntityByUniId(string uniId)
        {
            logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
            var userEntity = await userManagementService.GetUserByUniIdAsync(uniId);

            if (userEntity == null)
            {
                return NotFound(new {message = "User not found", messageCode = "user-not-found"});
            }

            logger.LogInformation($"User with UNI-ID {uniId} fetched successfully");
            return Ok(userEntity);
        }
        
        [Authorize]
        [HttpGet("Id/{id}")]
        public async Task<IActionResult> GetUserEntity(int id)
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

            logger.LogInformation($"User with ID {id} fetched successfully");
            return Ok(userEntity);
        }

        [Authorize]
        [HttpDelete("Delete/UniId/{uniId}")]
        public async Task<IActionResult> DeleteUserEntity(string uniId)
        {
            logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
            var userEntity = await userManagementService.GetUserByUniIdAsync(uniId);
            if (userEntity == null)
            {
                return NotFound(new {message = "User not found", messageCode = "user-not-found"});
            }
            
            var tokenUniId = User.FindFirst(ClaimTypes.UserData)?.Value;
            if (userEntity.UniId != tokenUniId)
            {
                return Unauthorized(new {message = "User not accessible", messageCode = "user-not-accessible"});
            }
            
            if (await userManagementService.DeleteUserAsync(userEntity))
            {
                logger.LogInformation($"User with UNI-ID {uniId} deleted successfully");
                return Ok(new { message = "User deleted successfully" });
            }
            
            return BadRequest(new {message = "User delete failed", messageCode = "user-delete-failed"});
        }
    }
}
