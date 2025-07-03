using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using App.DAL.EF;
using App.Domain;
using Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace App.BLL;

public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;
    private readonly UserRepository _userRepository;
    private readonly AuthRepository _authRepository;
    private readonly Initializer _initializer;
    
    public AuthService(AppDbContext context, ILogger<AuthService> logger, Initializer initializer)
    {
        _logger = logger;
        _userRepository = new UserRepository(context);
        _authRepository = new AuthRepository(context);
        _initializer = initializer;
    }
    
    public string GenerateJwtToken(UserEntity user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtKey = _initializer.jwtKey;
        var issuer = _initializer.jwtIssuer;
        var audience = _initializer.jwtAudience;
        
        var jwtExiprationDays = 7; // TODO: ENV!


        if (string.IsNullOrWhiteSpace(jwtKey))
        {
            _logger.LogError("Reading data from env failed (JWTKEY)");
            return string.Empty;
        }

        if (string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
        {
            _logger.LogError("Reading data from env failed (JWTISS or JWTAUD)");
            return string.Empty;
        }

        var key = Encoding.ASCII.GetBytes(jwtKey);

        var claims = new List<Claim>
        {
            new Claim("userId", user.Id.ToString()),
            new Claim("accessLevel", user.UserType?.AccessLevel.ToString() ?? "0"),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.ToUniversalTime().AddDays(jwtExiprationDays),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
    public async Task<string?> GenerateRefreshToken(Guid userId, string creatorIp)
    {
        var refreshExpirationDays = 7; // TODO: ENV!
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var now = DateTime.Now.ToUniversalTime();
        var tokenEntity = new RefreshTokenEntity()
        {
            UserId = userId,
            Token = token,
            ExpirationTime = DateTime.Now.ToUniversalTime().AddDays(refreshExpirationDays),
            CreatedByIp = creatorIp,
            CreatedBy = "aspnet-auth",
            CreatedAt = now,
            UpdatedBy = "aspnet-auth",
            UpdatedAt = now,
        };

        if (!await _authRepository.AddRefreshTokenAsync(tokenEntity))
        {
            _logger.LogError($"Refresh token creation failed for user with ID: {userId}");
            return null;
        }
        
        _logger.LogInformation($"Refresh token creation successfully for user with ID: {userId}");
        return token;
    }
    
    public async Task<(string? JwtToken, string? RefreshToken)> RefreshJwtToken(string refreshToken, string ipAddress)
    {
        var tokenEntity = await _authRepository.GetRefreshTokenAsync(refreshToken);

        if (tokenEntity == null || tokenEntity.ExpirationTime < DateTime.Now.ToUniversalTime())
        {
            _logger.LogWarning("Invalid or expired refresh token");
            return (null, null);
        }
    
        if (tokenEntity.IsUsed || tokenEntity.IsRevoked)
        {
            _logger.LogWarning("Refresh token already used or revoked");
            return (null, null);
        }

        var user = await _userRepository.GetUserByIdAsync(tokenEntity.UserId);

        if (user == null)
        {
            _logger.LogWarning("User not found for refresh token");
            return (null, null);
        }

        tokenEntity.IsUsed = true;
        tokenEntity.IsRevoked = true;
        tokenEntity.RevokedAt = DateTime.Now.ToUniversalTime();
        tokenEntity.RevokedByIp = ipAddress;
        tokenEntity.UpdatedAt = DateTime.Now.ToUniversalTime();
        tokenEntity.UpdatedBy = "aspnet-auth";

        var newRefreshTokenString = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var now = DateTime.Now.ToUniversalTime();
        var newRefreshToken = new RefreshTokenEntity()
        {
            UserId = user.Id,
            Token = newRefreshTokenString,
            ExpirationTime = now.AddDays(7),
            CreatedByIp = ipAddress,
            CreatedAt = now,
            CreatedBy = "aspnet-auth",
            UpdatedAt = now,
            UpdatedBy = "aspnet-auth",
            ReplacedByTokenId = null,
        };

        await _authRepository.AddRefreshTokenAsync(newRefreshToken);

        tokenEntity.ReplacedByTokenId = newRefreshToken.Id;
        await _authRepository.UpdateRefreshTokenAsync(tokenEntity);

        var jwt = GenerateJwtToken(user);

        return (jwt, newRefreshTokenString);
    }

}