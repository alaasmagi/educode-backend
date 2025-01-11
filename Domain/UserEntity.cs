namespace Domain;

public class UserEntity
{
    public int Id { get; set; }
    public int UserTypeId { get; set; }
    public string UniId { get; set; } = default!;
    public string? MatriculationNumber { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
}