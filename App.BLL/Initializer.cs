using App.DAL.EF;
using App.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace App.BLL;

public class Initializer
{
    private readonly ILogger<Initializer> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    public string OtpKey { get; private set; } = string.Empty;
    public string AdminUser { get; private set; } = string.Empty;
    public string AdminKey { get; private set; } = string.Empty;
    public string AdminTokenSalt { get; private set; } = string.Empty;
    public string JwtKey { get; private set; } = string.Empty;
    public string JwtAudience { get; private set; } = string.Empty;
    public string JwtIssuer { get; private set; } = string.Empty;
    public string MailSenderEmail { get; private set; } = string.Empty;
    public string MailSenderKey { get; private set; } = string.Empty;
    public string MailSenderHost { get; private set; } = string.Empty;
    public string MailSenderPort { get; private set; } = string.Empty;
    
    public Initializer(ILogger<Initializer> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }
    
    public void InitializeDb()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var attendanceRepository = new AttendanceRepository(context);
        var courseRepository = new CourseRepository(context);
        var userRepository = new UserRepository(context);

        attendanceRepository.SeedAttendanceTypes();
        courseRepository.SeedCourseStatuses();
        userRepository.SeedUserTypes();

        _logger.LogInformation("Database initialization completed.");
    }

    public void InitializeEnv()
    {
        OtpKey = Environment.GetEnvironmentVariable("OTPKEY") ?? string.Empty;
        
        AdminUser = Environment.GetEnvironmentVariable("ADMINUSER") ?? string.Empty;
        AdminKey = Environment.GetEnvironmentVariable("ADMINKEY") ?? string.Empty;
        AdminTokenSalt = Environment.GetEnvironmentVariable("ADMINTOKENSALT") ?? string.Empty;
        
        JwtKey = Environment.GetEnvironmentVariable("JWTKEY") ?? string.Empty;
        JwtAudience = Environment.GetEnvironmentVariable("JWTAUD") ?? string.Empty;
        JwtIssuer = Environment.GetEnvironmentVariable("JWTISS") ?? string.Empty;
        
        MailSenderEmail = Environment.GetEnvironmentVariable("MAILSENDER_EMAIL") ?? string.Empty;
        MailSenderKey = Environment.GetEnvironmentVariable("MAILSENDER_KEY") ?? string.Empty;
        MailSenderHost = Environment.GetEnvironmentVariable("MAILSENDER_HOST") ?? string.Empty;
        MailSenderPort = Environment.GetEnvironmentVariable("MAILSENDER_PORT") ?? string.Empty;
    }
}