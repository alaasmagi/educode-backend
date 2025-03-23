using App.Domain;
using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.ApiControllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IUserManagementService userManagementService,
    IAuthService authService,
    IOtpService otpService,
    IEmailService emailService,
    ILogger<AuthController> logger)
    : ControllerBase
{

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        var user = await userManagementService.GetUserByUniIdAsync(model.UniId);

        if (user == null)
        {
            return NotFound(new {message = "User not found", error = "user-not-found"});
        }
        
        var userAuthData = await userManagementService.AuthenticateUserAsync(user.Id, model.Password);
        if (userAuthData == null || !ModelState.IsValid)
        {
            logger.LogWarning($"Form data is invalid");
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
        
        logger.LogInformation($"User with UNI-ID {model.UniId} was logged in successfully");
        return Ok(new { Token = token });
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] CreateAccountModel model)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        var userType = await userManagementService.GetUserTypeAsync(model.UserRole);
        var newUser = new UserEntity();
        var newUserAuth = new UserAuthEntity();

        if (userType == null || !ModelState.IsValid)
        {
            logger.LogWarning($"Form data is invalid");
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

        newUserAuth.PasswordHash = userManagementService.GetPasswordHash(model.Password);

        if (!await userManagementService.CreateAccountAsync(newUser, newUserAuth))
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
        
        logger.LogInformation($"User with UNI-ID {model.UniId} was created successfully");
        return Ok(new { Token = token });
    }

    [HttpPost("RequestOTP")]
    public async Task<IActionResult> RequestOtp([FromBody] RequestOtpModel model)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        if (!ModelState.IsValid)
        {
            logger.LogWarning($"Form data is invalid");
            return BadRequest(new { message = "Invalid credentials", error = "invalid-credentials" });
        }

        var user = await userManagementService.GetUserByUniIdAsync(model.UniId);
        if (user == null || string.IsNullOrWhiteSpace(model.FullName))
        {
            return Unauthorized(new { message = "Invalid UNI-ID", error = "invalid-uni-id" });
        }

        var key = otpService.GenerateTotp(user?.UniId ?? model.UniId);
        var recipientUniId = user?.UniId ?? model.UniId;
        var recipientName = user?.FullName ?? model.FullName;

        await emailService.SendEmailAsync(recipientUniId, recipientName, key);
        
        logger.LogInformation($"OTP sent successfully for user with UNI-ID {model.UniId}");
        return Ok(new { message = "OTP sent successfully" });
    }

    [HttpPost("VerifyOTP")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpModel model)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        
        if (!ModelState.IsValid)
        {
            logger.LogWarning($"Form data is invalid");
            return BadRequest(new { message = "Invalid credentials", error = "invalid-credentials" });
        }
        
        var user = await userManagementService.GetUserByUniIdAsync(model.UniId);
        var result = otpService.VerifyTotp(model.UniId, model.Otp);

        if (!result)
        {
            return Unauthorized(new { message = "Invalid OTP", error = "invalid-otp" });
        }

        var token = string.Empty;
        if (user != null)
        {
            token = authService.GenerateJwtToken(user);
            Response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                MaxAge = TimeSpan.FromDays(60)
            });
        }
        
        if (token != string.Empty)
        {
            logger.LogInformation($"OTP verified successfully");
            return Ok(new { Token = token });
        }
        
        logger.LogInformation($"OTP verified successfully for user with UNI-ID {model.UniId}");
        return Ok();
    }

    [Authorize]
    [HttpPatch("ChangePassword")]
    public async Task<IActionResult> ChangeAccountPassword([FromBody] ChangePasswordModel model)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        if (!ModelState.IsValid)
        {
            logger.LogWarning($"Form data is invalid");
            return BadRequest(new { message = "Invalid credentials", error = "invalid-credentials" });
        }

        var user = await userManagementService.GetUserByUniIdAsync(model.UniId);

        if (user == null)
        {
            return Unauthorized(new { message = "Invalid UNI-ID", error = "invalid-uni-id" });
        }

        var newPasswordHash = userManagementService.GetPasswordHash(model.NewPassword);

        if (!await userManagementService.ChangeUserPasswordAsync(user, newPasswordHash))
        {
            return BadRequest(new
                { message = "Password change error. Password was not changed.", error = "password-not-changed" });
        }

        logger.LogInformation($"Password changed successfully for user with UNI-ID {model.UniId}");
        return Ok(new { message = "Password is changed successfully" });
    }
}
