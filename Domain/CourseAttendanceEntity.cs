namespace Domain;

public class CourseAttendanceEntity
{
    public int Id { get; set; }
    public int TeacherId { get; set; }
    public string CourseCode { get; set; } = default!;
    public int AttendanceTypeId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool OnlineRegistration { get; set; }
}