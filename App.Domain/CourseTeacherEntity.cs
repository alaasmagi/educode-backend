namespace App.Domain;

public class CourseTeacherEntity : BaseEntity
{
    public string CourseCode { get; set; } = default!;
    public int TeacherId { get; set; }
}