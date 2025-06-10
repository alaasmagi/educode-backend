using App.Domain;

namespace App.DTO;

public class UserDto(UserEntity user)
{
    public Guid Id { get; set; } = user.Id;
    public string UniId { get; set; } = user.UniId;
    public Guid UserTypeId { get; set; } = user.UserTypeId;
    public string? UserType = user.UserType?.UserType;
    public string? StudentCode = user.StudentCode;
}