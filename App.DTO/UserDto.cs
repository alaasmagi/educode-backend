using App.Domain;

namespace App.DTO;

public class UserDto(UserEntity user)
{
    public Guid Id { get; set; } = user.Id;
    public string UniId { get; set; } = user.UniId;
    public Guid UserTypeId { get; set; } = user.UserTypeId;
    public string? UserType = user.UserType?.UserType;
    public string? StudentCode = user.StudentCode;
    
    public static List<UserDto> ToDtoList(List<UserEntity>? entities)
    {
        if (entities == null)
        {
            return new List<UserDto>();
        }
        return entities.Select(e => new UserDto(e)).ToList();
    }
}