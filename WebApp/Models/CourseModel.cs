using App.Domain;

namespace WebApp.Models;

public class CourseModel
{
    public Guid? Id { get; set; }
    public required string UniId {get; set;}
    public required string CourseName { get; set; }
    public required string CourseCode { get; set; }
    public required Guid CourseStatusId { get; set; }
    public required string Creator { get; set; }
}