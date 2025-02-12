using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF;

public class CloudDataConnection : IDataConnection
{
    public string DatabaseUser { get; set; }
    public string DatabaseKey { get; set; }
    public string ConnectionString { get; set; }
    public DbContextOptions<AppDbContext> ContextOptions { get; set; }
}