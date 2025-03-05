using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using App.BLL;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using WebApp.Models;

namespace WebApp.ApiControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManagement userManagement;
        private readonly AuthBrain authService;
        private readonly EmailSender emailService;

        public UserController(AppDbContext context, IConfiguration configuration, EmailSender emailSender)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            var config = configuration ?? throw new ArgumentNullException(nameof(configuration));
        
            userManagement = new UserManagement(_context);
            authService = new AuthBrain(config);
            emailService = emailSender;
        }
        
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await userManagement.AuthenticateUser(model.UniId, model.Password);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid UNI-ID or password" });
            }

            var token = authService.GenerateJwtToken(user);
            return Ok(new { Token = token });
        }
        
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] CreateAccountModel model)
        {
            UserTypeEntity? userType = await userManagement.GetUserType(model.UserRole);
            UserEntity newUser = new UserEntity();
            UserAuthEntity newUserAuth = new UserAuthEntity();
            
            if (userType == null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            
            newUser.UniId = model.UniId;
            newUser.FullName = model.Fullname;
            newUser.StudentCode = model.StudentCode;
            newUser.UserTypeId = userType.Id;
            newUser.UserType = userType;
            newUser.CreatedBy = model.Creator;
            newUser.UpdatedBy = model.Creator;
            newUserAuth.CreatedBy = model.Creator;
            newUserAuth.UpdatedBy = model.Creator;
            
            newUserAuth.PasswordHash = userManagement.GetPasswordHash(model.Password);

            if (!await userManagement.CreateAccount(newUser, newUserAuth))
            {
                return BadRequest();
            }
            
            var token = authService.GenerateJwtToken(newUser);
            return Ok(new { Token = token });
        }
        
        // GET: api/User
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserEntity>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }
        
        // GET: api/User/UniId/<uni-id>
        [Authorize]
        [HttpGet("UniId/{uniId}")]
        public async Task<IActionResult> GetUserEntityByUniId(string uniId)
        {
            var userEntity = await _context.Users.Include(x => x.UserType)
                .FirstOrDefaultAsync(x => x.UniId == uniId);

            if (userEntity == null)
            {
                return NotFound();
            }

            return Ok(userEntity);
        }
        
        // GET: api/User/Id/5
        [Authorize]
        [HttpGet("Id/{id}")]
        public async Task<IActionResult> GetUserEntity(int id)
        {
            var userEntity = await _context.Users.FindAsync(id);

            if (userEntity == null)
            {
                return NotFound();
            }

            return Ok(userEntity);
        }

        [HttpPost("RequestOTP")]
        public async Task<IActionResult> RequestOtp(string uniId)
        {
         
            var user = await userManagement.GetUserByUniId(uniId);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid UNI-ID" });
            }

            var key = authService.GenerateOtp();
            var token = authService.GenerateJwtToken(user);
            
            await emailService.SendEmail(user, key);
            return Ok(new { Key = key, Token = token });

        }

        [Authorize]
        [HttpPatch("ChangePassword")]
        public async Task<IActionResult> ChangeAccountPassword([FromBody] ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var user = await userManagement.GetUserByUniId(model.UniId);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid UNI-ID" });
            }
            
            var newPasswordHash = userManagement.GetPasswordHash(model.NewPassword);

            if (await userManagement.ChangeUserPassword(user, newPasswordHash))
            {
                return Ok(new { message = "Password changed successfully" });
            }
            return BadRequest(new { message = "Invalid credentials" });
        }

        // DELETE: api/User/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserEntity(int id)
        {
            var userEntity = await _context.Users.FindAsync(id);
            if (userEntity == null)
            {
                return NotFound();
            }

            _context.Users.Remove(userEntity);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
