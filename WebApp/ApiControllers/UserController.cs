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
        private readonly OtpBrain otpService;

        public UserController(AppDbContext context, IConfiguration configuration, EmailSender emailSender)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            var config = configuration ?? throw new ArgumentNullException(nameof(configuration));
        
            userManagement = new UserManagement(_context);
            authService = new AuthBrain(config);
            otpService = new OtpBrain();
            emailService = emailSender;
        }

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
                return NotFound(new {message = "User not found", error = "user-not-found"});
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
                return NotFound(new {message = "User not found", error = "user-not-found"});
            }

            return Ok(userEntity);
        }
        
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await userManagement.AuthenticateUser(model.UniId, model.Password);
            if (user == null || !ModelState.IsValid)
            {
                return Unauthorized(new { message = "Invalid UNI-ID or password", error = "invalid-uni-id-password" });
            }

            var token = authService.GenerateJwtToken(user);
            Response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = true,    
                Secure = true,     
                SameSite = SameSiteMode.Strict, 
                MaxAge = TimeSpan.FromDays(60)  
            });
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
                return BadRequest(new { message = "Invalid credentials", error = "invalid-credentials" });
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
                return BadRequest(new { message = "User already exists", error = "user-already-exists" });
            }
            
            var token = authService.GenerateJwtToken(newUser);
            Response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = true,    
                Secure = true,     
                SameSite = SameSiteMode.Strict, 
                MaxAge = TimeSpan.FromDays(60)  
            });
            return Ok(new { Token = token });
        }

        [HttpPost("RequestOTP")]
        public async Task<IActionResult> RequestOtp([FromBody] RequestOtpModel model)
        {
            var user = await userManagement.GetUserByUniId(model.UniId);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid UNI-ID", error = "invalid-uni-id" });
            }

            var key = otpService.GenerateTOTP(user.UniId);
            
            await emailService.SendEmail(user, key);
            return Ok();
        }

        [HttpPost("VerifyOTP")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpModel model)
        {
            var user = await userManagement.GetUserByUniId(model.UniId);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid UNI-ID", error = "invalid-uni-id" });
            }
            
            var result = otpService.VerifyTOTP(user.UniId, model.Otp);

            if (!result)
            {
                return Unauthorized(new {message = "Invalid OTP", error = "invalid-otp"});
            }
            
            var token = authService.GenerateJwtToken(user);
            Response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = true,    
                Secure = true,     
                SameSite = SameSiteMode.Strict, 
                MaxAge = TimeSpan.FromDays(60)  
            });
            return Ok(new { Token = token });
        }

        [Authorize]
        [HttpPatch("ChangePassword")]
        public async Task<IActionResult> ChangeAccountPassword([FromBody] ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new {message = "Invalid credentials", error = "invalid-credentials"});;
            }
            
            var user = await userManagement.GetUserByUniId(model.UniId);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid UNI-ID", error = "invalid-uni-id" });
            }
            
            var newPasswordHash = userManagement.GetPasswordHash(model.NewPassword);

            if (!await userManagement.ChangeUserPassword(user, newPasswordHash))
            {
                return BadRequest(new { message = "Password change error. Password was not changed.", error = "password-not-changed"});
            }
            
            return Ok();

        }

        // DELETE: api/User/5
        [Authorize]
        [HttpDelete("Delete/{uniId}")]
        public async Task<IActionResult> DeleteUserEntity(string uniId)
        {
            var userEntity = await _context.Users.FirstOrDefaultAsync(u => u.UniId == uniId);
            if (userEntity == null)
            {
                return NotFound(new {message = "User not found", error = "user-not-found"});
            }

            _context.Users.Remove(userEntity);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
