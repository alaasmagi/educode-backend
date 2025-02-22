using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Base.Domain;

namespace App.Domain;

public class UserAuthEntity : BaseEntity
{
    [ForeignKey("User")]
    public int UserId { get; set; }
    public UserEntity? User { get; set; }
    [MaxLength(255)]
    public string PasswordHash { get; set; } = default!;
}