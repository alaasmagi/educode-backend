using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class WorkplaceEntity : BaseEntity
{
    [MaxLength(128)]
    public string ClassRoom { get; set; } = default!;
    [MaxLength(128)]
    public string ComputerCode { get; set; } = default!;
}