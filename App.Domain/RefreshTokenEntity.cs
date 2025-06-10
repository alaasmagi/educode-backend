using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class RefreshTokenEntity : BaseEntity
{
    [Required]
    public Guid UserId { get; set; }
    public UserEntity? User { get; set; }
    [Required]
    [MaxLength(512)]
    public string Token { get; set; } = default!;
    [Required]
    public DateTime ExpirationTime { get; set; }
    [Required]
    public bool IsUsed { get; set; } = false;
    [Required]
    public bool IsRevoked { get; set; } = false;
    public Guid? ReplacedByTokenId { get; set; }
    public DateTime? RevokedAt { get; set; }
    [MaxLength(128)]
    public string? RevokedByIp { get; set; }
    [Required]
    [MaxLength(128)]
    public string CreatedByIp { get; set; } = default!;
}