using App.Domain;

namespace App.DAL.EF;

public class AuthRepository(AppDbContext context)
{
    public async Task<bool> AddRefreshToken(RefreshTokenEntity refreshToken)
    {
        context.RefreshTokens.Add(refreshToken);
        return await context.SaveChangesAsync() > 0;
    }
}