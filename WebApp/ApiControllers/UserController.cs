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
            logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path} - " +
                                $"{HttpContext.Request.Host.ToString()}");
           return await userManagementService.GetAllUsersAsync();
        }
        
        [Authorize]
        [HttpGet("UniId/{uniId}")]
        public async Task<IActionResult> GetUserEntityByUniId(string uniId)
        {
            logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path} - " +
                                  $"{HttpContext.Request.Host.ToString()}");
            var userEntity = await userManagementService.GetUserByUniIdAsync(uniId);

            if (userEntity == null)
            {
                return NotFound(new {message = "User not found", error = "user-not-found"});
            }

            return Ok(userEntity);
        }
        
        [Authorize]
        [HttpGet("Id/{id}")]
        public async Task<IActionResult> GetUserEntity(int id)
        {
            logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path} - " +
                                  $"{HttpContext.Request.Host.ToString()}");
            var userEntity = await userManagementService.GetUserByIdAsync(id);

            if (userEntity == null)
            {
                return NotFound(new {message = "User not found", error = "user-not-found"});
            }

            return Ok(userEntity);
        }

        [Authorize]
        [HttpDelete("Delete/UniId/{uniId}")]
        public async Task<IActionResult> DeleteUserEntity(string uniId)
        {
            logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path} - " +
                                  $"{HttpContext.Request.Host.ToString()}");
            var userEntity = await userManagementService.GetUserByUniIdAsync(uniId);
            if (userEntity == null)
            {
                return NotFound(new {message = "User not found", error = "user-not-found"});
            }
            
            await userManagementService.DeleteUserAsync(userEntity);
            return Ok(new { message = "User deleted successfully" });
        }
    }
}
