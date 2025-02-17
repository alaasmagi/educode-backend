using App.DAL.EF;
using App.Domain;

namespace App.BLL;

public class UserManagement(AppDbContext context)
{
    public bool AddUser(UserEntity newUser)
    {
        context.Users.Add(newUser);
        context.SaveChanges();
        return true;
    }
}