namespace App.Domain;

public class CourseUserCountDto
{
    public DateTime AttendanceDate { get; set; }
    public int UserCount { get; set; } = 0;
}