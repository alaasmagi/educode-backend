using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Base.Domain;

namespace App.Domain;

public class UserEntity : BaseEntity
{
    [Required]
    [ForeignKey("UserType")]
    public int? UserTypeId { get; set; }
    public UserTypeEntity? UserType { get; set; }
    [Required]
    [MaxLength(128)]
    public string UniId { get; set; } = default!;
    [Required]
    [MaxLength(128)]
    public string? StudentCode { get; set; }
    [Required]
    [MaxLength(255)]
    public string FullName { get; set; } = default!;
}