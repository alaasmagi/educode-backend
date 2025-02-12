using System.ComponentModel.DataAnnotations.Schema;
using Base.Domain;

namespace App.Domain;

public class AttendanceCheckEntity : BaseEntity
{
    public int StudentId { get; set; }
    public UserEntity Student { get; set; } = default!;
    
    public int CourseAttendanceId { get; set; }
    public CourseAttendanceEntity CourseAttendance { get; set; } = default!;
    
    public int? WorkplaceId { get; set; }
    public WorkplaceEntity? Workplace { get; set; }
}