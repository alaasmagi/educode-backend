using Base.Domain;

namespace App.Domain;

public class CourseEntity : BaseEntity
{
    public string CourseCode { get; set; } = default!;
    public string CourseName { get; set; } = default!;
    public ECourseValidStatus CourseValidStatus { get; set; }
}