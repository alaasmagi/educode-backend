using App.DAL.EF;
using App.Domain;

namespace App.BLL;

public class UserManagement
{
    private static AppDbContext _context = default!;
    public UserManagement(AppDbContext context)
    {
        _context = context;
    }
    private static readonly UserRepository User = new UserRepository(_context);
    
    public UserEntity? AuthenticateUser(string username, string password)
    {
        var userAuthData = User.GetUserAuthEntityByUsername(username);

        if (userAuthData == null)
        {
            return null;
        }

        return VerifyPasswordHash(password, userAuthData.PasswordHash) ? userAuthData.User! : null;
    }
    
    private bool VerifyPasswordHash(string enteredPassword, string storedHash)
    {
        return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);
    }
}