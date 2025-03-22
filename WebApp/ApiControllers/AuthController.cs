using App.Domain;
using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.ApiControllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    ICourseManagementService courseManagementService,
    IUserManagementService userManagementService,
    IAuthService authService,
    IOtpService otpService,
    IEmailService emailService)
    : ControllerBase
{

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await userManagementService.AuthenticateUserAsync(model.UniId, model.Password);
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
        UserTypeEntity? userType = await userManagementService.GetUserTypeAsync(model.UserRole);
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
        return Ok(new { Token = token });
    }

    [HttpPost("RequestOTP")]
    public async Task<IActionResult> RequestOtp([FromBody] RequestOtpModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { message = "Invalid credentials", error = "invalid-credentials" });
        }

        var user = await userManagementService.GetUserByUniIdAsync(model.UniId);
        if (user == null && string.IsNullOrWhiteSpace(model.FullName))
        {
            return Unauthorized(new { message = "Invalid UNI-ID", error = "invalid-uni-id" });
        }

        var key = otpService.GenerateTotp(user?.UniId ?? model.UniId);
        var recipientUniId = user?.UniId ?? model.UniId;
        var recipientName = user?.FullName ?? model.FullName;

        await emailService.SendEmailAsync(recipientUniId, recipientName, key);
        return Ok(new { message = "OTP sent successfully" });
    }

    [HttpPost("VerifyOTP")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpModel model)
    {
        var user = await userManagementService.GetUserByUniIdAsync(model.UniId);

        if (user == null)
        {
            return Unauthorized(new { message = "Invalid UNI-ID", error = "invalid-uni-id" });
        }

        var result = otpService.VerifyTotp(user.UniId, model.Otp);

        if (!result)
        {
            return Unauthorized(new { message = "Invalid OTP", error = "invalid-otp" });
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
            return BadRequest(new { message = "Invalid credentials", error = "invalid-credentials" });
            ;
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

        return Ok(new { message = "Password is changed successfully" });
    }
}
