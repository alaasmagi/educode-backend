using System.Security.Claims;
using App.Domain;

namespace Contracts;

public interface IAuthService
{
    string GenerateJwtToken(UserEntity user);
    Task<string?> GenerateRefreshToken(Guid userId, string creatorIp);
    Task<(string? JwtToken, string? RefreshToken)> RefreshJwtToken(string refreshToken, string jwtToken, string ipAddress);
    Guid? GetUserIdFromJwt(string jwtToken);
    Task<bool> VerifyRefreshToken(string refreshToken, Guid userId, string ipAddress);
    Task<bool> DeleteRefreshToken(string refreshToken);
}