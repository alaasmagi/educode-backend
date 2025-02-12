using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Base.Domain;

namespace App.Domain;

public class CourseEntity : BaseEntity
{
    [MaxLength(128)]
    public string CourseCode { get; set; } = default!;
    [MaxLength(128)]
    public string CourseName { get; set; } = default!;
    public ECourseValidStatus CourseValidStatus { get; set; }

    public ICollection<CourseTeacherEntity> CourseTeacherEntities { get; set; } = new List<CourseTeacherEntity>();
    public ICollection<CourseAttendanceEntity> CourseAttendanceEntities { get; set; } = new List<CourseAttendanceEntity>();
}