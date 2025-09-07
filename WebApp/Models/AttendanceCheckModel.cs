namespace WebApp.Models;

public class AttendanceCheckModel
{
    public required string StudentCode { get; set; }
    public required string FullName { get; set; }
    public required string CourseAttendanceIdentifier { get; set; }
    public string? WorkplaceIdentifier { get; set; }
    public required string Creator { get; set; }
}