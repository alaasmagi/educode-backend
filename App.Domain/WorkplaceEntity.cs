using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class WorkplaceEntity : BaseEntity
{
    [Required]
    public int Identifier { get; set; }
    [Required]
    [MaxLength(128)]
    public string ClassRoom { get; set; } = default!;
    [Required]
    [MaxLength(128)]
    public string ComputerCode { get; set; } = default!;
}