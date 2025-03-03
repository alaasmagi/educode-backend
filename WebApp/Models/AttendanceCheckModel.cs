namespace WebApp.Models;

public class AttendanceCheckModel
{
    public required string StudentCode { get; set; }
    public required int CourseAttendanceId { get; set; }
    public int? WorkplaceId { get; set; }
    public required string Creator { get; set; }
}