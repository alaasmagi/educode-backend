using App.DAL.EF;
using App.Domain;

namespace App.BLL;

public class UserManagement
{
    private readonly AppDbContext _context;
    private readonly UserRepository User;

    public UserManagement(AppDbContext context)
    {
        _context = context;
        User = new UserRepository(_context); 
    }
    
    public UserEntity? AuthenticateUser(string uniId, string password)
    {
        var userAuthData = User.GetUserAuthEntityByUniIdOrStudentCode(uniId);

        if (userAuthData == null)
        {
            return null;
        }

        return VerifyPasswordHash(password, userAuthData.PasswordHash) ? userAuthData.User! : null;
    }

    public bool CreateAccount(UserEntity user, UserAuthEntity userAuthData)
    {
        if (DoesUserExist(user.UniId))
        {
            return false;
        }
        
        User.AddUserEntityToDb(user);
        userAuthData.UserId = user.Id;
        User.AddUserAuthEntityToDb(userAuthData);

        return true;
    }
    
    private bool VerifyPasswordHash(string enteredPassword, string storedHash)
    {
        return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);
    }

    private bool DoesUserExist(string uniId)
    {
        return _context.Users.Any(u => u.UniId == uniId);
    }

    public UserTypeEntity? GetUserType (string userType)
    {
        return _context.UserTypes.FirstOrDefault(u => u.UserType == userType) ?? null;
    }
    
    public string GetPasswordHash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }
}