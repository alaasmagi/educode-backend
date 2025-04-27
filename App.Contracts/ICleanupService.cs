using Microsoft.Extensions.Hosting;
namespace Contracts;


public interface ICleanupService : IHostedService, IDisposable
{
    Task HardDeleteOldAttendanceEntitiesAsync();
    Task HardDeleteOldAttendanceCheckEntitiesAsync();
    Task HardDeleteOldUserAuthEntitiesAsync();
    Task HardDeleteOldUserEntitiesAsync();
    Task HardDeleteOldCourseTeacherEntitiesAsync();
    Task HardDeleteOldCourseEntitiesAsync();
}