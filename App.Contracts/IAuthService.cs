using System.Security.Claims;
using App.Domain;

namespace Contracts;

public interface IAuthService
{
    string GenerateJwtToken(UserEntity user);    
}