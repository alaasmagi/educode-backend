using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class UserTypeEntity : BaseEntity
{
    [Required]
    [MaxLength(128)]
    public string UserType { get; set; } = default!;
    
}