using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Base.Domain;

namespace App.Domain;

public class UserAuthTokenEntity : BaseEntity
{
    [ForeignKey("User")]
    public int UserId { get; set; }
    public UserEntity User { get; set; } = default!;
    [MaxLength(255)]
    public string Token { get; set; } = default!;
    public DateTime ExpireTime { get; set; }
}