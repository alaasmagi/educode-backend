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
    public async Task<IActionResult> Login([FromBody] LoginRequestModel requestModel)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        var user = await userManagementService.GetUserByEmailAsync(requestModel.Email);

        if (user == null)
        {
            return NotFound(new {message = "User not found", messageCode = "user-not-found"});
        }
        
        var userAuthData = await userManagementService.AuthenticateUserAsync(user.Id, requestModel.Password);
        if (userAuthData == null || !ModelState.IsValid)
        {
            logger.LogWarning($"Form data is invalid");
            return Unauthorized(new { message = "Invalid email or password", messageCode = "invalid-email-password" });
        }

        var jwtToken = authService.GenerateJwtToken(user);
        var creatorIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var refreshToken = await authService.GenerateRefreshToken(user.Id, creatorIp);

        if (refreshToken == null)
        {
            logger.LogWarning($"Refresh token generation failed");
            return BadRequest(new { message = "Refresh token generation failed", messageCode = "refresh-token-error" });
        }
        
        Response.Cookies.Append("token", jwtToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            MaxAge = TimeSpan.FromMinutes(15) // TODO: ENV!
        });
        
        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,                
            Secure = true,                 
            SameSite = SameSiteMode.None,   
            MaxAge = TimeSpan.FromDays(15) // TODO: ENV!
        });
        
        logger.LogInformation($"User with ID {user.Id} was logged in successfully");
        return Ok(new { Token = jwtToken, RefreshToken = refreshToken});
    }
    
    [HttpPost("Refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestModel model)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");

        if (!ModelState.IsValid)
        {
            logger.LogWarning($"Form data is invalid");
            return BadRequest(new { message = "Invalid credentials", messageCode = "invalid-credentials" });
        }

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var (newJwt,  newRefreshToken) = await authService.RefreshJwtToken(model.RefreshToken, model.JwtToken, ipAddress);

        if (newJwt == null || newRefreshToken == null)
        {
            return Unauthorized(new { message = "Invalid or expired refresh token", messageCode = "invalid-refresh-token" });
        }

        return Ok(new { Token = newJwt, RefreshToken = newRefreshToken });
    }
    
    [HttpPost("Logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestModel model)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
    
        if (!ModelState.IsValid)
        {
            logger.LogWarning($"Form data is invalid");
            return BadRequest(new { message = "Invalid credentials", messageCode = "invalid-credentials" });
        }
        
        var status = await authService.DeleteRefreshToken(model.RefreshToken);

        if (status == false)
        {
            logger.LogWarning($"Logging out failed");
            return BadRequest(new { message = "Logging out failed", messageCode = "logout-failed" });
        }

        Response.Cookies.Delete("token");
        Response.Cookies.Delete("refreshToken");

        return Ok(new { message = "Logged out successfully", messageCode = "logout-successful" });
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] CreateAccountRequestModel requestModel)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        var userType = await userManagementService.GetUserTypeAsync(requestModel.UserRole);
        var newUser = new UserEntity();
        var newUserAuth = new UserAuthEntity();

        if (userType == null || !ModelState.IsValid)
        {
            logger.LogWarning($"Form data is invalid");
            return BadRequest(new { message = "Invalid credentials", messageCode = "invalid-credentials" });
        }

        newUser.Email = requestModel.Email;
        newUser.FullName = requestModel.Fullname;
        newUser.StudentCode = requestModel.StudentCode;
        newUser.UserTypeId = userType.Id;
        newUser.CreatedBy = requestModel.Creator;
        newUser.UpdatedBy = requestModel.Creator;
        
        newUserAuth.CreatedBy = requestModel.Creator;
        newUserAuth.UpdatedBy = requestModel.Creator;
        newUserAuth.PasswordHash = userManagementService.GetPasswordHash(requestModel.Password);

        if (!await userManagementService.CreateAccountAsync(newUser, newUserAuth))
        {
            return BadRequest(new { message = "User already exists", messageCode = "user-already-exists" });
        }
        
        logger.LogInformation($"User with email {newUser.Email} was created successfully");
        return Ok();
    }

    [HttpPost("RequestOTP")]
    public async Task<IActionResult> RequestOtp([FromBody] RequestOtpModel model)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        if (!ModelState.IsValid)
        {
            logger.LogWarning($"Form data is invalid");
            return BadRequest(new { message = "Invalid credentials", messageCode = "invalid-credentials" });
        }

        var user = await userManagementService.GetUserByEmailAsync(model.Email);
        if (user == null && string.IsNullOrWhiteSpace(model.FullName))
        {
            return Unauthorized(new { message = "Invalid email", messageCode = "invalid-email" });
        }

        var key = await otpService.GenerateAndStoreOtp(model.Email);
        var recipientEmail = user?.Email ?? model.Email;
        var recipientName = user?.FullName ?? model.FullName ?? "EduCode user";

        if (!await emailService.SendEmailAsync(recipientEmail, recipientName, key))
        {
            return BadRequest(new { message = "Email was not sent", messageCode = "email-was-not-sent" });
        }
        
        logger.LogInformation($"OTP sent successfully for user with email {model.Email}");
        return Ok(new { message = "OTP sent successfully" });
    }

    [HttpPost("VerifyOTP")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpModel model)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        
        if (!ModelState.IsValid)
        {
            logger.LogWarning($"Form data is invalid");
            return BadRequest(new { message = "Invalid credentials", messageCode = "invalid-credentials" });
        }
        
        var user = await userManagementService.GetUserByEmailAsync(model.Email);
        
        var result = await otpService.VerifyOtp(model.Email, model.Otp);

        if (!result)
        {
            return Unauthorized(new { message = "Invalid OTP", messageCode = "invalid-otp" });
        }

        if (user != null)
        {
            var token = authService.GenerateJwtToken(user);
            Response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                MaxAge = TimeSpan.FromMinutes(15) // TODO: ENV!
            });
            
            var creatorIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var refreshToken = await authService.GenerateRefreshToken(user.Id, creatorIp);
            Response.Cookies.Append("refreshToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                MaxAge = TimeSpan.FromDays(15) // TODO: ENV!
            });
                
            logger.LogInformation($"OTP verified successfully for user with email {user.Email}");
            return Ok(new { Token = token });
        }
        
        logger.LogInformation($"OTP verified successfully");
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
            return BadRequest(new { message = "Invalid credentials", messageCode = "invalid-credentials" });
        }

        var user = await userManagementService.GetUserByEmailAsync(model.Email);

        if (user == null)
        {
            return Unauthorized(new { message = "Invalid email", messageCode = "invalid-email" });
        }

        var newPasswordHash = userManagementService.GetPasswordHash(model.NewPassword);

        if (!await userManagementService.ChangeUserPasswordAsync(user, newPasswordHash))
        {
            return BadRequest(new { message = "Password change error. Password was not changed.", messageCode = "password-not-changed" });
        }

        logger.LogInformation($"Password changed successfully for user with email {model.Email}");
        return Ok(new { message = "Password is changed successfully" });
    }
}
