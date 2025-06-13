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
    private readonly AppDbContext _context;
    public readonly UserRepository _userRepository;
    public readonly AuthRepository _authRepository;
    
    public AuthService(AppDbContext context, ILogger<AuthService> logger)
    {
        _logger = logger;
        _context = context;
        _userRepository = new UserRepository(_context);
        _authRepository = new AuthRepository(_context);
    }
    
    public string GenerateJwtToken(UserEntity user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtKey = Environment.GetEnvironmentVariable("JWTKEY");
        var issuer = Environment.GetEnvironmentVariable("JWTISS");
        var audience = Environment.GetEnvironmentVariable("JWTAUD");
        
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
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.UserData, user.UniId),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.UserType?.UserType ?? "User")
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(jwtExiprationDays),
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
            ExpirationTime = DateTime.UtcNow.AddDays(refreshExpirationDays),
            CreatedByIp = creatorIp,
            CreatedBy = "aspnet-auth",
            CreatedAt = now,
            UpdatedBy = "aspnet-auth",
            UpdatedAt = now,
        };

        if (!await _authRepository.AddRefreshToken(tokenEntity))
        {
            _logger.LogError($"Refresh token creation failed for user with ID: {userId}");
            return null;
        }
        
        _logger.LogInformation($"Refresh token creation successfully for user with ID: {userId}");
        return token;
    }
}