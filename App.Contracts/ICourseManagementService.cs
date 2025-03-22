using App.Domain;

namespace Contracts;

public interface ICourseManagementService
{
    Task<CourseEntity?> GetCourseByAttendanceIdAsync(int attendanceId);
}