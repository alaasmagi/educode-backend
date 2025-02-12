using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF;

public interface IDataConnection
{
    public string DatabaseUser { get; set; }
    public string DatabaseKey { get; set; }
    public static string Location { get; set; } = default!;
    public string ConnectionString { get; set; }
    public DbContextOptions<AppDbContext> ContextOptions { get; set; } 
}