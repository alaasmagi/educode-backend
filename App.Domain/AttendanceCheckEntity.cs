using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Base.Domain;

namespace App.Domain;

public class AttendanceCheckEntity : BaseEntity
{
    [Required]
    public string StudentCode { get; set; } = default!;
    [Required]
    public int CourseAttendanceId { get; set; }
    public int? WorkplaceId { get; set; }
    public WorkplaceEntity? Workplace { get; set; }
}