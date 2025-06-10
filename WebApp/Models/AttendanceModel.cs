using App.Domain;

namespace WebApp.Models;

public class AttendanceModel
{
    public Guid? Id { get; set; }
    public required Guid CourseId {get; set;}
    public required Guid AttendanceTypeId { get; set; }
    public required TimeOnly StartTime  { get; set; }
    public required TimeOnly EndTime  { get; set; }
    public required List<DateOnly> AttendanceDates { get; set; }
    public required string Creator { get; set; }
}