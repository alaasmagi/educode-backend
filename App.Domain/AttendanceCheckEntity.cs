using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class AttendanceCheckEntity : BaseEntity
{
    [Required]
    public string StudentCode { get; set; } = default!;
    [Required]
    [MaxLength(255)]
    public string FullName { get; set; } = default!;
    [Required]
    public int AttendanceIdentifier { get; set; }
    public int? WorkplaceIdentifier { get; set; }
    public WorkplaceEntity? Workplace { get; set; }
}