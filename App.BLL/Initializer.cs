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

    public string otpKey { get; private set; } = string.Empty;
    
    public string adminUser { get; private set; } = string.Empty;
    public string adminKey { get; private set; } = string.Empty;
    public string adminTokenSalt { get; private set; } = string.Empty;
    
    public string jwtKey { get; private set; } = string.Empty;
    public string jwtAudience { get; private set; } = string.Empty;
    public string jwtIssuer { get; private set; } = string.Empty;
    
    public string mailSenderEmail { get; private set; } = string.Empty;
    public string mailSenderKey { get; private set; } = string.Empty;
    public string mailSenderHost { get; private set; } = string.Empty;
    public string mailSenderPort { get; private set; } = string.Empty;
    
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
        otpKey = Environment.GetEnvironmentVariable("OTPKEY") ?? string.Empty;
        
        adminUser = Environment.GetEnvironmentVariable("ADMINUSER") ?? string.Empty;
        adminKey = Environment.GetEnvironmentVariable("ADMINKEY") ?? string.Empty;
        adminTokenSalt = Environment.GetEnvironmentVariable("ADMINTOKENSALT") ?? string.Empty;
        
        jwtKey = Environment.GetEnvironmentVariable("JWTKEY") ?? string.Empty;
        jwtAudience = Environment.GetEnvironmentVariable("JWTAUD") ?? string.Empty;
        jwtIssuer = Environment.GetEnvironmentVariable("JWTISS") ?? string.Empty;
        
        mailSenderEmail = Environment.GetEnvironmentVariable("MAILSENDER_EMAIL") ?? string.Empty;
        mailSenderKey = Environment.GetEnvironmentVariable("MAILSENDER_KEY") ?? string.Empty;
        mailSenderHost = Environment.GetEnvironmentVariable("MAILSENDER_HOST") ?? string.Empty;
        mailSenderPort = Environment.GetEnvironmentVariable("MAILSENDER_PORT") ?? string.Empty;
    }
}