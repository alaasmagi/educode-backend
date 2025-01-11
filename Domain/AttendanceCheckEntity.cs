namespace Domain;

public class AttendanceCheckEntity
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int CourseAttendanceId { get; set; }
    public int? WorkplaceId { get; set; }
}