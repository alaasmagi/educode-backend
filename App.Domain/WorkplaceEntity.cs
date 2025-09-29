using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class WorkplaceEntity : BaseEntity
{
    [Required] 
    public string Identifier { get; set; } = default!;
    [Required]
    public Guid ClassRoomId { get; set; }
    public ClassroomEntity? ClassRoom { get; set; }
    [Required]
    [MaxLength(128)]
    public string ComputerCode { get; set; } = default!;
    [Required]
    public Guid SchoolId { get; set; }
    public SchoolEntity? School { get; set; }
}