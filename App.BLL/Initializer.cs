using App.DAL.EF;
using App.Domain;
using Microsoft.Extensions.Logging;

namespace App.BLL;

public class Initializer
{
    private readonly ILogger<Initializer> _logger;
    private readonly AppDbContext _context;
    private readonly AttendanceRepository _attendanceRepository;
    private readonly CourseRepository _courseRepository;
    private readonly UserRepository _userRepository;

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
    
    public Initializer(AppDbContext context, ILogger<Initializer> logger)
    {
        _logger = logger;
        _context = context;
        _attendanceRepository = new AttendanceRepository(_context);
        _courseRepository = new CourseRepository(_context);
        _userRepository = new UserRepository(_context);
    }
    
    public void InitializeDb()
    {
        _attendanceRepository.SeedAttendanceTypes();
        _courseRepository.SeedCourseStatuses();
        _userRepository.SeedUserTypes();
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