using App.Domain;

namespace WebApp.Models;

public class AttendanceModel
{
    public int? Id { get; set; }
    public required string CourseCode {get; set;}
    public required string AttendanceTypeId { get; set; }
    public required TimeOnly StartTime  { get; set; }
    public required TimeOnly EndTime  { get; set; }
    public required List<DateOnly> AttendanceDates { get; set; }
    public required string Creator { get; set; }
}