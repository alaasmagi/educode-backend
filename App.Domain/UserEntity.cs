namespace App.Domain;

public class UserEntity : BaseEntity
{
    public int UserTypeId { get; set; }
    public string UniId { get; set; } = default!;
    public string? MatriculationNumber { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
}