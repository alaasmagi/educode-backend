using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using App.Domain;
using Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace App.BLL;

public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;
    
    public AuthService(ILogger<AuthService> logger)
    {
        _logger = logger;
    }
    
    public string GenerateJwtToken(UserEntity user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtKey = Environment.GetEnvironmentVariable("JWTKEY");
        
        if (jwtKey == null)
        {
            _logger.LogError("Reading data from env failed (JWTKEY)");
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

        var issuer = Environment.GetEnvironmentVariable("JWTISS");
        var audience = Environment.GetEnvironmentVariable("JWTAUD");
        
        if (issuer == null || audience == null)
        {
            _logger.LogError("Reading data from env failed (JWTISS or JWTAUD)");
            return string.Empty;
        }
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(60),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public ClaimsPrincipal? ValidateJwtToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtKey = Environment.GetEnvironmentVariable("JWTKEY");
        
        if (jwtKey == null)
        {
            _logger.LogError("Reading data from env failed (JWTKEY)");
            return null;
        }
        
        var key = Encoding.ASCII.GetBytes(jwtKey);
        
        var issuer = Environment.GetEnvironmentVariable("JWTISS");
        var audience = Environment.GetEnvironmentVariable("JWTAUD");
        
        if (issuer == null || audience == null)
        {
            _logger.LogError("Reading data from env failed (JWTISS or JWTAUD)");
            return null;
        }
        
        var parameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true
        };

        var principal = tokenHandler.ValidateToken(token, parameters, out _);
        return principal;
    }
    
}