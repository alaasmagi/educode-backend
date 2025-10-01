using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using App.DAL.EF;
using App.Domain;
using Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace App.BLL;

public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;
    private readonly UserRepository _userRepository;
    private readonly RedisRepository _redisRepository;
    private readonly EnvInitializer _envInitializer;
    
    public AuthService(AppDbContext context, ILogger<AuthService> logger, EnvInitializer envInitializer, IConnectionMultiplexer connectionMultiplexer, ILogger<RedisRepository> redisLogger)
    {
        _logger = logger;
        _userRepository = new UserRepository(context);
        _redisRepository = new RedisRepository(connectionMultiplexer, redisLogger);
        _envInitializer = envInitializer;
    }
    
    public string GenerateJwtToken(UserEntity user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtKey = _envInitializer.JwtKey;
        var issuer = _envInitializer.JwtIssuer;
        var audience = _envInitializer.JwtAudience;
        
        var jwtExpirationMinutes = _envInitializer.JwtExpirationMinutes;
        var now = DateTime.UtcNow;

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
            new Claim(Constants.UserIdClaim, user.Id.ToString()),
            new Claim(Constants.AccessLevelClaim, ((int)(user.UserType?.AccessLevel ?? EAccessLevel.NoAccess)).ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = now.AddMinutes(jwtExpirationMinutes),
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
        var refreshTokenExpirationDays = _envInitializer.RefreshTokenExpirationDays;
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        
        var tokenData = new RefreshTokenEntity
        {
            UserId = userId,
            Token = token,
            CreatedByIp = creatorIp,
        };
        var json = JsonSerializer.Serialize(tokenData);
        
        if (!await _redisRepository.SetDataAsync(Constants.RefreshTokenPrefix + token, json, TimeSpan.FromDays(refreshTokenExpirationDays)))
        {
            _logger.LogError($"Refresh token creation failed for user with ID: {userId}");
            return null;
        }
        
        _logger.LogInformation($"Refresh token creation successfully for user with ID: {userId}");
        return token;
    }
    
    public async Task<(string? JwtToken, string? RefreshToken)> RefreshJwtToken(string refreshToken, string jwtToken, string ipAddress)
    {
        var userId = GetUserIdFromJwt(jwtToken);
        if (userId == null)
        {
            _logger.LogError("JWT validation failed");
            return (null, null);
        }

        if (!await VerifyRefreshToken(jwtToken, userId.Value, ipAddress))
        {
            _logger.LogError($"Refresh token validation for user with ID {userId} failed");
            return (null, null);
        }

        var user = await _userRepository.GetUserByIdAsync(userId.Value);
        var newJwtToken = GenerateJwtToken(user!);
        
        await _redisRepository.DeleteDataAsync(Constants.RefreshTokenPrefix + refreshToken);
        var newRefreshToken = await GenerateRefreshToken(userId.Value, ipAddress);
        
        _logger.LogInformation($"JWT and refresh tokens successfully refreshed for user with ID: {userId}");
        return (newJwtToken, newRefreshToken);
    }
    
    public Guid? GetUserIdFromJwt(string jwtToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwtToken);
        var userIdClaim = token.Claims.FirstOrDefault(c => c.Type == "userId");
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        
        return null;
    }
    
    public async Task<bool> VerifyRefreshToken(string refreshToken, Guid userId, string ipAddress)
    {
        var serializedToken = await _redisRepository.GetDataAsync(Constants.RefreshTokenPrefix + refreshToken);
        
        if (serializedToken == null)
        {
            return false;
        }
        
        var tokenEntity = JsonSerializer.Deserialize<RefreshTokenEntity>(serializedToken);
        
        if (tokenEntity == null)
        {
            return false;
        }

        if (tokenEntity.UserId != userId || tokenEntity.Token != refreshToken || tokenEntity.CreatedByIp != ipAddress)
        {
            return false;
        }
        
        return true;
    }

    public async Task<bool> DeleteRefreshToken(string refreshToken)
    {
        if (!await _redisRepository.DeleteDataAsync(Constants.RefreshTokenPrefix + refreshToken))
        {
            _logger.LogError($"Refresh token deletion failed");
            return false;
        }
        
        _logger.LogInformation($"Refresh token deletion successfully");
        return true;
    }

}