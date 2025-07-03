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
        var user = await userManagementService.GetUserByUniIdAsync(requestModel.UniId);

        if (user == null)
        {
            return NotFound(new {message = "User not found", messageCode = "user-not-found"});
        }
        
        var userAuthData = await userManagementService.AuthenticateUserAsync(user.Id, requestModel.Password);
        if (userAuthData == null || !ModelState.IsValid)
        {
            logger.LogWarning($"Form data is invalid");
            return Unauthorized(new { message = "Invalid UNI-ID or password", messageCode = "invalid-uni-id-password" });
        }

        var token = authService.GenerateJwtToken(user);
        Response.Cookies.Append("token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            MaxAge = TimeSpan.FromDays(60)
        });
        
        logger.LogInformation($"User with ID {user.Id} was logged in successfully");
        return Ok(new { Token = token });
    }
    
    [HttpPost("Refresh")]
public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestModel model)
{
    logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");

    if (string.IsNullOrWhiteSpace(model.RefreshToken))
        return BadRequest(new { message = "Refresh token is required", messageCode = "refresh-token-required" });

    var (newJwt,  newRefreshToken) = await authService.RefreshJwtToken(model.RefreshToken, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown");

    if (newJwt == null || newRefreshToken == null)
    {
        return Unauthorized(new { message = "Invalid or expired refresh token", messageCode = "invalid-refresh-token" });
    }

    return Ok(new { Token = newJwt });
}
    
    [HttpPost("Logout")]
    public async Task<IActionResult> Logout([FromBody] string refreshToken)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        var userType = await userManagementService.GetUserTypeAsync(model.UserRole);
        var newUser = new UserEntity();
        var newUserAuth = new UserAuthEntity();

        if (userType == null || !ModelState.IsValid)
        {
            logger.LogWarning($"Form data is invalid");
            return BadRequest(new { message = "Invalid credentials", messageCode = "invalid-credentials" });
        }

        newUser.UniId = model.UniId;
        newUser.FullName = model.Fullname;
        newUser.StudentCode = model.StudentCode;
        newUser.UserTypeId = userType.Id;
        newUser.CreatedBy = model.Creator;
        newUser.UpdatedBy = model.Creator;
        newUserAuth.CreatedBy = model.Creator;
        newUserAuth.UpdatedBy = model.Creator;

        newUserAuth.PasswordHash = userManagementService.GetPasswordHash(model.Password);

        if (!await userManagementService.CreateAccountAsync(newUser, newUserAuth))
        {
            return BadRequest(new { message = "User already exists", messageCode = "user-already-exists" });
        }
        
        logger.LogInformation($"User with UNI-ID {model.UniId} was created successfully");
        return Ok();
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

        newUser.UniId = requestModel.UniId;
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
        
        logger.LogInformation($"User with UNI-ID {newUser.Id} was created successfully");
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

        var user = await userManagementService.GetUserByUniIdAsync(model.UniId);
        if (user == null && string.IsNullOrWhiteSpace(model.FullName))
        {
            return Unauthorized(new { message = "Invalid UNI-ID", messageCode = "invalid-uni-id" });
        }

        var key = otpService.GenerateTotp(user?.UniId ?? model.UniId);
        var recipientUniId = user?.UniId ?? model.UniId;
        var recipientName = user?.FullName ?? model.FullName ?? "EduCode user";

        if (!await emailService.SendEmailAsync(recipientUniId, recipientName, key))
        {
            return BadRequest(new { message = "Email was not sent", messageCode = "email-was-not-sent" });
        }
        
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
            return BadRequest(new { message = "Invalid credentials", messageCode = "invalid-credentials" });
        }
        
        var user = await userManagementService.GetUserByUniIdAsync(model.UniId);
        
        var result = otpService.VerifyTotp(model.UniId, model.Otp);

        if (!result)
        {
            return Unauthorized(new { message = "Invalid OTP", messageCode = "invalid-otp" });
        }

        var token = string.Empty;
        if (user != null)
        {
            token = authService.GenerateJwtToken(user);
            Response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
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
            return BadRequest(new { message = "Invalid credentials", messageCode = "invalid-credentials" });
        }

        var user = await userManagementService.GetUserByUniIdAsync(model.UniId);

        if (user == null)
        {
            return Unauthorized(new { message = "Invalid UNI-ID", messageCode = "invalid-uni-id" });
        }

        var newPasswordHash = userManagementService.GetPasswordHash(model.NewPassword);

        if (!await userManagementService.ChangeUserPasswordAsync(user, newPasswordHash))
        {
            return BadRequest(new { message = "Password change error. Password was not changed.", messageCode = "password-not-changed" });
        }

        logger.LogInformation($"Password changed successfully for user with UNI-ID {model.UniId}");
        return Ok(new { message = "Password is changed successfully" });
    }
}
