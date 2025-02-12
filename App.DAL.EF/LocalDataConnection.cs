using System.Data.Common;
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF;

public class LocalDataConnection : IDataConnection
{
    public string DatabaseUser { get; set; } = default!;
    public string DatabaseKey { get; set; } = default!;
    public static string Location { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) +
                                           Path.DirectorySeparatorChar + "educode-backend" +
                                           Path.DirectorySeparatorChar + "app.db";

    public string ConnectionString { get; set; } = $"Data Source={Location}";
    public DbContextOptions<AppDbContext> ContextOptions { get; set; } = new DbContextOptionsBuilder<AppDbContext>().
                                                        UseSqlite(Location).EnableDetailedErrors().
                                                        EnableSensitiveDataLogging().Options;
}