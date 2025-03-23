using App.Domain;

namespace WebApp.Models;

public class CourseModel
{
    public int? Id { get; set; }
    public required string UniId {get; set;}
    public required string CourseName { get; set; }
    public required string CourseCode { get; set; }
    public required ECourseValidStatus Status { get; set; }
    public required string Creator { get; set; }
}