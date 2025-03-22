using Contracts;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

public class BaseController(IAdminAccessService adminAccessService) : Controller
{
    public async Task<bool> IsTokenValidAsync(HttpContext httpContext)
    {
        var sessionToken = httpContext.Session.GetString("token");
        var hashedSessionToken = httpContext.Session.GetString("token-hash");

        if (string.IsNullOrEmpty(sessionToken) || string.IsNullOrEmpty(hashedSessionToken))
        {
            return false;
        }
        return await  adminAccessService.CompareHashedTokensAsync(sessionToken, hashedSessionToken);
    }

    public async Task SetTokensAsync()
    {
        var sessionToken = adminAccessService.GenerateAdminAccessToken();
        var hashedSessionToken = await adminAccessService.GetHashedAdminAccessTokenAsync(sessionToken);
        
        HttpContext.Session.SetString("token", sessionToken);
        HttpContext.Session.SetString("token-hash", hashedSessionToken);
    }
}