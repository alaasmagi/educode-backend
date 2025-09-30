using App.DAL.EF;
using App.Domain;
using Contracts;
using Microsoft.Extensions.Logging;

namespace App.BLL;

public class SchoolManagementService : ISchoolManagementService
{
    private readonly AppDbContext _context;
    private readonly SchoolRepository _schoolRepository;
    private readonly ILogger<SchoolManagementService> _logger;

    public SchoolManagementService(AppDbContext context, ILogger<SchoolManagementService> logger)
    {
        _logger = logger;
        _context = context;
        _schoolRepository = new SchoolRepository(_context);
    }

    public async Task<List<SchoolEntity>?> GetAllSchools()
    {
        var schools = await _schoolRepository.GetAllSchools();

        return schools.Count > 0 ? schools : null;
    }

    public async Task<SchoolEntity?> GetSchoolById(Guid id)
    {
        var school = await _schoolRepository.GetSchoolById(id);

        return school;
    }
}