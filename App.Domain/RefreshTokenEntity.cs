using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class RefreshTokenEntity : BaseEntity
{
    [Required]
    public Guid UserId { get; set; }
    [Required]
    public string Token { get; set; } = default!; 
    [Required]
    public string CreatedByIp { get; set; } = default!;
    [Required]
    public DateTime ExpirationTime { get; set; }
}