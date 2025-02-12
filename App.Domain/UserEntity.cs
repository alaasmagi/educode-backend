using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Base.Domain;

namespace App.Domain;

public class UserEntity : BaseEntity
{
    [ForeignKey("UserType")]
    public int? UserTypeId { get; set; }
    public UserTypeEntity? UserType { get; set; }
    [MaxLength(128)]
    public string UniId { get; set; } = default!;
    [MaxLength(128)]
    public string? MatriculationNumber { get; set; }
    [MaxLength(128)]
    public string FirstName { get; set; } = default!;
    [MaxLength(128)]
    public string LastName { get; set; } = default!;

    public ICollection<AttendanceCheckEntity>? AttendanceChecks { get; set; }= new List<AttendanceCheckEntity>();
    public ICollection<UserAuthTokenEntity>? UserAuthTokens { get; set; } = new List<UserAuthTokenEntity>();
    public ICollection<CourseTeacherEntity>? CourseTeachers { get; set; } = new List<CourseTeacherEntity>();
}