using App.BLL;
using App.DAL.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace App.BLL_Tests;

[TestFixture]
public class AttendanceManagementServiceTests
{
    private AppDbContext _context;
    private AttendanceManagementService _service;
    private ILogger<AttendanceManagementService> _logger;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        _context = new AppDbContext(options);
        _logger = new LoggerFactory().CreateLogger<AttendanceManagementService>();
        _service = new AttendanceManagementService(_context, _logger);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}