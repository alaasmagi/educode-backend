using App.Domain;

namespace Contracts;

public interface ICourseManagementService
{
    Task<CourseEntity?> GetCourseByAttendanceIdAsync(int attendanceId);
    Task<CourseEntity?> GetCourseByIdAsync(int courseId);
    Task<bool> AddCourse(UserEntity user, CourseEntity course, string creator);
    Task<bool> EditCourse(int courseId, CourseEntity newCourse);
    Task<bool> DeleteCourse(int courseId);
    List<CourseStatusDto> GetAllCourseStatuses();
    Task<List<CourseEntity>> GetCoursesByUserAsync(string uniId);
    Task<List<CourseUserCountDto>?> GetAttendancesUserCountsByCourseAsync(int courseId);
    Task<bool> DoesCourseExistAsync(int id);
}