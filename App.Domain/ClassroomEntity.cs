using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class ClassroomEntity : BaseEntity
{
    [Required]
    [MaxLength(128)]
    public string ClassRoom { get; set; } = default!;
    public ICollection<CourseAttendanceEntity>? CourseAttendances { get; set; }
}