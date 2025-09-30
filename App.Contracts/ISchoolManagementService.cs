using App.Domain;

namespace Contracts;

public interface ISchoolManagementService
{
    Task<List<SchoolEntity>?> GetAllSchools();
    Task<SchoolEntity?> GetSchoolById(Guid id);
}