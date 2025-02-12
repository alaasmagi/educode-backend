using Base.Domain;

namespace App.Domain;

public class UserTypeEntity : BaseEntity
{
    public string UserType { get; set; } = default!;
}