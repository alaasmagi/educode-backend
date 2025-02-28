using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Base.Domain;

namespace App.Domain;

public class CourseEntity : BaseEntity
{
    [Required]
    [MaxLength(128)]
    public string CourseCode { get; set; } = default!;
    [Required]
    [MaxLength(128)]
    public string CourseName { get; set; } = default!;
    [Required]
    public ECourseValidStatus CourseValidStatus { get; set; }
    public ICollection<CourseTeacherEntity>? CourseTeacherEntities { get; set; }
}