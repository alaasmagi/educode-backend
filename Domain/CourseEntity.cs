namespace Domain;

public class CourseEntity
{
    public int Id { get; set; }
    public string CourseCode { get; set; } = default!;
    public string CourseName { get; set; } = default!;
    public ECourseValidStatus CourseValidStatus { get; set; }
}