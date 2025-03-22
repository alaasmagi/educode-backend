using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using App.Domain;
using Contracts;

namespace WebApp.ApiControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(
        ICourseManagementService courseManagementService,
        IUserManagementService userManagementService,
        IAuthService authService,
        IOtpService otpService,
        IEmailService emailService)
        : ControllerBase
    {

        [HttpGet("TestConnection")]
        public ActionResult TestConnection()
        {
            return Ok();
        }
        
        // GET: api/User
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserEntity>>> GetUsers()
        {
           return await userManagementService.GetAllUsersAsync();
        }
        
        // GET: api/User/UniId/<uni-id>
        [Authorize]
        [HttpGet("UniId/{uniId}")]
        public async Task<IActionResult> GetUserEntityByUniId(string uniId)
        {
            var userEntity = await userManagementService.GetUserByUniIdAsync(uniId);

            if (userEntity == null)
            {
                return NotFound(new {message = "User not found", error = "user-not-found"});
            }

            return Ok(userEntity);
        }
        
        // GET: api/User/Id/5
        [Authorize]
        [HttpGet("Id/{id}")]
        public async Task<IActionResult> GetUserEntity(int id)
        {
            var userEntity = await userManagementService.GetUserByIdAsync(id);

            if (userEntity == null)
            {
                return NotFound(new {message = "User not found", error = "user-not-found"});
            }

            return Ok(userEntity);
        }

        // DELETE: api/User/5
        [Authorize]
        [HttpDelete("Delete/{uniId}")]
        public async Task<IActionResult> DeleteUserEntity(string uniId)
        {
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
