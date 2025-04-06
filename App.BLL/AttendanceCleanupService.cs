using App.DAL.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class AttendanceCleanupService : IHostedService, IDisposable
{
    private readonly ILogger<AttendanceCleanupService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private Timer _timer;

    public AttendanceCleanupService(ILogger<AttendanceCleanupService> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var timeToRun = DateTime.Today.AddDays(1).AddHours(3) - DateTime.Now;
        _timer = new Timer(ExecuteTask, null, timeToRun, TimeSpan.FromDays(1));
        _logger.LogInformation("Attendance cleanup service started.");
        return Task.CompletedTask;
    }

    private async void ExecuteTask(object state)
    {
        try
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await DeleteOldAttendances(context);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred during the attendance cleanup task: {ex.Message}");
        }
    }

    private async Task<bool> DeleteOldAttendances(AppDbContext context)
    {
        var tempRepository = new AttendanceRepository(context);
        var status = await tempRepository.RemoveOldAttendances();

        if (!status)
        {
            _logger.LogInformation($"Found no attendances to delete that are more than 6 months old");
            return false;
        }

        _logger.LogInformation($"Successfully deleted attendances that were more than 6 months old");
        return true;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer.Change(Timeout.Infinite, 0);
        _logger.LogInformation("Attendance cleanup service stopped.");
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer.Dispose();
        _logger.LogInformation("Attendance cleanup service disposed.");
    }
}