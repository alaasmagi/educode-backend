using App.Domain;

namespace Contracts;

public interface IUserManagementService
{
    Task<UserEntity?> AuthenticateUserAsync(string uniId, string password);
    Task<bool> CreateAccountAsync(UserEntity user, UserAuthEntity userAuthData);
    Task<bool> ChangeUserPasswordAsync(UserEntity user, string newPasswordHash);
    Task<UserEntity?> GetUserByUniIdAsync(string uniId);
    Task<UserTypeEntity?> GetUserTypeAsync(string userType);
    Task<List<UserEntity>> GetAllUsersAsync();
    Task<bool> DoesUserExistAsync(string uniId);
    Task<UserEntity?> GetUserByIdAsync(int id);
    string GetPasswordHash(string password);
    Task<bool> DeleteUserAsync(UserEntity user);
}