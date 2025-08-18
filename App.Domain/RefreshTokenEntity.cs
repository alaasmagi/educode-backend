using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class RefreshTokenEntity
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = default!; 
    public string CreatedByIp { get; set; } = default!;
}