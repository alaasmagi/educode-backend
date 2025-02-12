using Base.Domain;

namespace App.Domain;

public class AttendanceCheckEntity : BaseEntity
{
    public int StudentId { get; set; }
    public int CourseAttendanceId { get; set; }
    public int? WorkplaceId { get; set; }
}