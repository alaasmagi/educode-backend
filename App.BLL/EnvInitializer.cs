using App.DAL.EF;
using App.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace App.BLL;

public class EnvInitializer
{
    private readonly ILogger<EnvInitializer> _logger;
    // DB
    public string PgDbConnection { get; private set; } = string.Empty;
    public string RedisConnection { get; private set; } = string.Empty;

    // JWT
    public string JwtKey { get; private set; } = string.Empty;
    public string JwtAudience { get; private set; } = string.Empty;
    public string JwtIssuer { get; private set; } = string.Empty;
    public int JwtExpirationMinutes { get; private set; }
    public int JwtCookieExpirationMinutes { get; private set; }

    // RefreshToken
    public int RefreshTokenExpirationDays { get; private set; }
    public int RefreshTokenCookieExpirationDays { get; private set; }
    
    // Admin
    public string AdminUser { get; private set; } = string.Empty;
    public string AdminKey { get; private set; } = string.Empty;
    public string AdminTokenSalt { get; private set; } = string.Empty;

    // OTP
    public string OtpKey { get; private set; } = string.Empty;
    public int OtpExpirationMinutes { get; private set; }

    // Mail
    public string MailSenderEmail { get; private set; } = string.Empty;
    public string MailSenderKey { get; private set; } = string.Empty;
    public string MailSenderHost { get; private set; } = string.Empty;
    public string MailSenderPort { get; private set; } = string.Empty;

    // Frontend
    public string FrontendUrl { get; private set; } = string.Empty;

    
    public EnvInitializer(ILogger<EnvInitializer> logger)
    {
        _logger = logger;
    }

    public void InitializeEnv()
    {
        PgDbConnection = GetStringEnv("PG_DB_CONNECTION");
        RedisConnection = GetStringEnv("REDIS_CONNECTION");

        OtpKey = GetStringEnv("OTPKEY");
        OtpExpirationMinutes = GetIntEnv("OTP_MINUTES");

        AdminUser = GetStringEnv("ADMINUSER");
        AdminKey = GetStringEnv("ADMINKEY");
        AdminTokenSalt = GetStringEnv("ADMINTOKENSALT");

        RefreshTokenExpirationDays = GetIntEnv("REFRESH_TOKEN_DAYS");
        RefreshTokenCookieExpirationDays = GetIntEnv("REFRESH_TOKEN_COOKIE_DAYS");

        JwtKey = GetStringEnv("JWTKEY");
        JwtAudience = GetStringEnv("JWTAUD");
        JwtIssuer = GetStringEnv("JWTISS");
        JwtExpirationMinutes = GetIntEnv("JWT_MINUTES");
        JwtCookieExpirationMinutes = GetIntEnv("JWT_COOKIE_MINUTES");

        MailSenderEmail = GetStringEnv("MAILSENDER_EMAIL");
        MailSenderKey = GetStringEnv("MAILSENDER_KEY");
        MailSenderHost = GetStringEnv("MAILSENDER_HOST");
        MailSenderPort = GetStringEnv("MAILSENDER_PORT");

        FrontendUrl = GetStringEnv("FRONTENDURL");

        _logger.LogInformation("Environment variables initialized.");
    }
    
    private string GetStringEnv(string key)
    {
        var value = Environment.GetEnvironmentVariable(key);
        if (string.IsNullOrWhiteSpace(value))
        {
            _logger.LogWarning($"Environment variable '{key}' is missing or empty.");
            return string.Empty;
        }
        return value;
    }

    private int GetIntEnv(string key)
    {
        var value = Environment.GetEnvironmentVariable(key);
        if (int.TryParse(value, out var result))
        {
            return result;
        }
        _logger.LogWarning($"Environment variable '{key}' is missing or not an integer. Using 0 as default.");
        return 0;
    }
}