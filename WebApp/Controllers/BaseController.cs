using App.BLL;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

public class BaseController : Controller
{
    private readonly AdminAccessManagement _access = new AdminAccessManagement();
    public bool IsTokenValid(HttpContext httpContext)
    {
        var sessionToken = httpContext.Session.GetString("token");
        var hashedSessionToken = httpContext.Session.GetString("token-hash");

        if (string.IsNullOrEmpty(sessionToken) || string.IsNullOrEmpty(hashedSessionToken))
        {
            return false;
        }
        bool kj = _access.CompareHashedTokens(sessionToken, hashedSessionToken);
        return _access.CompareHashedTokens(sessionToken, hashedSessionToken);
    }

    public void SetTokens()
    {
        var sessionToken = _access.GenerateAdminAccessToken();
        var hashedSessionToken = _access.GetHashedAdminAccessToken(sessionToken);
        
        HttpContext.Session.SetString("token", sessionToken);
        HttpContext.Session.SetString("token-hash", hashedSessionToken);
    }
}