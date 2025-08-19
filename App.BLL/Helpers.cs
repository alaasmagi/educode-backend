using App.Domain;
using Microsoft.AspNetCore.Authorization;

namespace App.BLL;

public class Helpers
{
    public static int GetAccessLevelFromClaims(AuthorizationHandlerContext context)
    {
        var claimValue = context.User.FindFirst(Constants.AccessLevelClaim)?.Value;
        return int.TryParse(claimValue, out var level) ? level : 0;
    }
}