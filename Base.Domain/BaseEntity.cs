using System.ComponentModel.DataAnnotations;

namespace Base.Domain;

public class BaseEntity
{
    public int Id { get; set; }
    [MaxLength(128)]
    public string CreatedBy { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    [MaxLength(128)]
    public string UpdatedBy { get; set; } = default!;
    public DateTime UpdatedAt { get; set; }
}