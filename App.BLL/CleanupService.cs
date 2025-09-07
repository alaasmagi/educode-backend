using App.DAL.EF;
using Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace App.BLL;

public class CleanupService : ICleanupService
{
    private readonly ILogger<CleanupService> _logger;
    private readonly AttendanceRepository _attendanceRepository;
    private readonly UserRepository _userRepository;
    private readonly CourseRepository _courseRepository;
    private readonly DateTime _datePeriod;
    private Timer? _timer;

    public CleanupService(ILogger<CleanupService> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _timer = null;
        var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        _attendanceRepository = new AttendanceRepository(dbContext);
        _userRepository = new UserRepository(dbContext);
        _courseRepository = new CourseRepository(dbContext);
        _datePeriod = DateTime.UtcNow.AddMonths(-6);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var timeToRun = DateTime.Today.AddDays(1).AddHours(3) - DateTime.UtcNow;
        _timer = new Timer(ExecuteTask!, null, timeToRun, TimeSpan.FromDays(1));
        _logger.LogInformation("Attendance cleanup service started.");
        return Task.CompletedTask;
    }

    private async void ExecuteTask(object state)
    {
        try
        {
            await HardDeleteOldAttendanceEntitiesAsync();
            await HardDeleteOldAttendanceCheckEntitiesAsync();
            await HardDeleteOldUserAuthEntitiesAsync();
            await HardDeleteOldUserEntitiesAsync();
            await HardDeleteOldCourseTeacherEntitiesAsync();
            await HardDeleteOldCourseEntitiesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred during the attendance cleanup task: {ex.Message}");
        }
    }

    public async Task HardDeleteOldCourseEntitiesAsync()
    {
        var status = await _courseRepository.RemoveOldCourses(_datePeriod);

        if (!status)
        {
            _logger.LogInformation($"Found no attendances to delete that are more than 6 months old");
        }

        _logger.LogInformation($"Successfully deleted attendances that were more than 6 months old");
    }
    
    public async Task HardDeleteOldUserEntitiesAsync()
    {
        var status = await _userRepository.RemoveOldUsers(_datePeriod);

        if (!status)
        {
            _logger.LogInformation($"Found no attendances to delete that are more than 6 months old");
        }

        _logger.LogInformation($"Successfully deleted attendances that were more than 6 months old");
    }
    
    public async Task HardDeleteOldUserAuthEntitiesAsync()
    {
        var status = await _userRepository.RemoveOldUserAuths(_datePeriod);

        if (!status)
        {
            _logger.LogInformation($"Found no attendances to delete that are more than 6 months old");
        }

        _logger.LogInformation($"Successfully deleted attendances that were more than 6 months old");
    }
    
    public async Task HardDeleteOldAttendanceCheckEntitiesAsync()
    {
        var status = await _attendanceRepository.RemoveOldAttendanceChecks(_datePeriod);

        if (!status)
        {
            _logger.LogInformation($"Found no attendances to delete that are more than 6 months old");
        }

        _logger.LogInformation($"Successfully deleted attendances that were more than 6 months old");
    }
    
    public async Task HardDeleteOldCourseTeacherEntitiesAsync()
    {
        var status = await _courseRepository.RemoveOldCourseTeachers(_datePeriod);

        if (!status)
        {
            _logger.LogInformation($"Found no attendances to delete that are more than 6 months old");
        }

        _logger.LogInformation($"Successfully deleted attendances that were more than 6 months old");
    }
    
    
    public async Task HardDeleteOldAttendanceEntitiesAsync()
    {
        var status = await _attendanceRepository.RemoveOldAttendances(_datePeriod);

        if (!status)
        {
            _logger.LogInformation($"Found no attendances to delete that are more than 6 months old");
        }
        
        _logger.LogInformation($"Successfully deleted attendances that were more than 6 months old");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        _logger.LogInformation("Attendance cleanup service stopped.");
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
        _logger.LogInformation("Attendance cleanup service disposed.");
    }
}