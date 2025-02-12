using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace App.DAL.EF;

public class AppDbContextFactory(IDataConnection dataConnection) : IDesignTimeDbContextFactory<AppDbContext>
{
    readonly IDataConnection _connection = dataConnection;
    public AppDbContext CreateDbContext(string[] args)
    {
        return new AppDbContext(_connection.ContextOptions);
    }
}