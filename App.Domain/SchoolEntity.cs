using Base.Domain;
using System.ComponentModel.DataAnnotations;


namespace App.Domain;

public class SchoolEntity : BaseEntity
{
    [Required]
    [MaxLength(256)]
    public string Name { get; set; } = default!;
    [Required]
    [MaxLength(128)]
    public string ShortName { get; set; } = default!;
    [Required]
    [MaxLength(256)]
    public string Domain { get; set; } = default!;
    [Required]
    [MaxLength(128)]
    public string StudentCodePattern { get; set; } = default!;
}