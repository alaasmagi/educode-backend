﻿using App.Domain;

namespace Contracts;

public interface IUserManagementService
{
    Task<UserEntity?> AuthenticateUserAsync(Guid userId, string password);
    Task<bool> CreateAccountAsync(UserEntity user, UserAuthEntity userAuthData);
    Task<bool> ChangeUserPasswordAsync(UserEntity user, string newPasswordHash);
    Task<UserEntity?> GetUserByUniIdAsync(string uniId);
    Task<UserTypeEntity?> GetUserTypeAsync(string userType);
    Task<List<UserEntity>?> GetAllUsersAsync();
    Task<bool> DoesUserExistAsync(string uniId);
    Task<UserEntity?> GetUserByIdAsync(Guid id);
    string GetPasswordHash(string password);
    Task<bool> DeleteUserAsync(UserEntity user);
}