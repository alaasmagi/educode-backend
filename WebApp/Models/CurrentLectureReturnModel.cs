namespace WebApp.Models;

public class CurrentLectureReturnModel
{
    public string CourseName { get; set; } = default!;
    public string CourseCode { get; set; } = default!;
    public int AttendanceId { get; set; }
}