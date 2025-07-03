using System.Security.Claims;
using App.Domain;

namespace Contracts;

public interface IAuthService
{
    string GenerateJwtToken(UserEntity user);
    Task<string?> GenerateRefreshToken(Guid userId, string creatorIp);
    Task<(string? JwtToken, string? RefreshToken)> RefreshJwtToken(string refreshToken, string ipAddress);
}