using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using App.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace App.BLL;

public class AuthBrain
{
    private readonly IConfiguration _config;
    
    public AuthBrain(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateJwtToken(UserEntity user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWTKEY")!);

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
            Expires = DateTime.UtcNow.AddDays(60),
            Issuer = Environment.GetEnvironmentVariable("JWTISS")!,
            Audience = Environment.GetEnvironmentVariable("JWTAUD")!,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public ClaimsPrincipal? ValidateJwtToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWTKEY")!);

        try
        {
            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = Environment.GetEnvironmentVariable("JWTISS")!,
                ValidateAudience = true,
                ValidAudience = Environment.GetEnvironmentVariable("JWTAUD")!,
                ValidateLifetime = true
            };

            var principal = tokenHandler.ValidateToken(token, parameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }

    public string GenerateOtp(string uniId)
    {
        string key = Environment.GetEnvironmentVariable("OTPKEY")!;
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 120;
        string data = $"{uniId}{timestamp}{key}";
    
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
        {
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            
            int offset = hash[hash.Length - 1] & 0x0F; 
            int otpBinary = (hash[offset] & 0x7F) << 24
                            | (hash[offset + 1] & 0xFF) << 16
                            | (hash[offset + 2] & 0xFF) << 8
                            | (hash[offset + 3] & 0xFF);

            int otp = otpBinary % 1000000; 
            return otp.ToString("D6");
        }
    }
    
    public bool VerifyOtp(string uniId, string userOtp)
    {
        string generatedOtp = GenerateOtp(uniId);
        
        Console.WriteLine("Generated OTP: " + generatedOtp);
        Console.WriteLine("User OTP: " + userOtp);
        return generatedOtp == userOtp;
    }
}