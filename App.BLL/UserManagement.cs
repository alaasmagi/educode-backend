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
        
        return password == userAuthData.PasswordHash ? userAuthData.User! : null;
    }
    
    private bool VerifyPasswordHash(string enteredPassword, string storedHash)
    {
        return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);
    }
}