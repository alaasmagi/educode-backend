using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Base.Domain;

namespace App.Domain;

public class CourseAttendanceEntity : BaseEntity
{
    [ForeignKey("Course")]
    public int CourseId { get; set; } = default!;
    public CourseEntity Course { get; set; } = default!;
    
    [ForeignKey("AttendanceType")]
    public int AttendanceTypeId { get; set; }
    public AttendanceTypeEntity AttendanceType { get; set; } = default!;
    
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool OnlineRegistration { get; set; }

    public ICollection<AttendanceCheckEntity>? AttendanceChecks { get; set; }
}