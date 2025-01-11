namespace Domain;

public class CourseTeacherEntity
{
    public int Id { get; set; }
    public string CourseCode { get; set; } = default!;
    public int TeacherId { get; set; }
}