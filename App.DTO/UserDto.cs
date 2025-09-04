using App.Domain;

namespace App.DTO;

public class UserDto(UserEntity user)
{
    public Guid Id { get; set; } = user.Id;
    public string UniId { get; set; } = user.UniId;
    public Guid UserTypeId { get; set; } = user.UserTypeId;
    public string? UserType { get; set; } = user.UserType?.UserType ?? null;
    public EAccessLevel? AccessLevel { get; set; } = user.UserType?.AccessLevel ?? null;
    public string? StudentCode { get; set; } = user.StudentCode ?? null;
    
    public static List<UserDto> ToDtoList(List<UserEntity>? entities)
    {
        if (entities == null)
        {
            return new List<UserDto>();
        }
        return entities.Select(e => new UserDto(e)).ToList();
    }
}