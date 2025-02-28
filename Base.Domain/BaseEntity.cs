using System.ComponentModel.DataAnnotations;

namespace Base.Domain;

public class BaseEntity
{
    [Required]
    public int Id { get; set; }
    [Required]
    [MaxLength(128)]
    public string CreatedBy { get; set; } = default!;
    [Required]
    public DateTime CreatedAt { get; set; }
    [Required]
    [MaxLength(128)]
    public string UpdatedBy { get; set; } = default!;
    [Required]
    public DateTime UpdatedAt { get; set; }
}