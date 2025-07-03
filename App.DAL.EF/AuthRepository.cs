using App.Domain;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF;

public class AuthRepository(AppDbContext context)
{
    public async Task<bool> AddRefreshTokenAsync(RefreshTokenEntity refreshToken)
    {
        context.RefreshTokens.Add(refreshToken);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<RefreshTokenEntity?> GetRefreshTokenAsync(string refreshToken)
    {
        return await context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshToken);
    }
    
    public async Task<bool> UpdateRefreshTokenAsync(RefreshTokenEntity token)
    {
        context.RefreshTokens.Update(token);
        return await context.SaveChangesAsync() > 0;
    }
}