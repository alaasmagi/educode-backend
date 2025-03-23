using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Base.Domain;

namespace App.Domain;

public class CourseAttendanceEntity : BaseEntity
{
    [Required]
    [ForeignKey("Course")]
    public int CourseId { get; set; }
    public CourseEntity? Course { get; set; }
    [Required]
    [ForeignKey("AttendanceType")]
    public int AttendanceTypeId { get; set; }
    public AttendanceTypeEntity? AttendanceType { get; set; }
    [Required]
    public DateTime StartTime { get; set; }
    [Required]
    public DateTime EndTime { get; set; }

    public ICollection<AttendanceCheckEntity>? AttendanceChecks { get; set; }
}