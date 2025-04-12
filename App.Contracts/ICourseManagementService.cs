using App.Domain;

namespace Contracts;

public interface ICourseManagementService
{
    Task<CourseEntity?> GetCourseByAttendanceIdAsync(int attendanceId);
    Task<bool> AddCourse(UserEntity user, CourseEntity course, string creator);
    Task<bool> EditCourse(int courseId, CourseEntity newCourse);
    Task<bool> DeleteCourse(int courseId, string uniId);
    List<CourseStatusDto>? GetAllCourseStatuses();
    Task<List<CourseEntity>?> GetCoursesByUserAsync(int userId);
    Task<List<CourseUserCountDto>?> GetAttendancesUserCountsByCourseAsync(int courseId);
    Task<CourseEntity?> GetCourseByNameAsync(string courseName, string uniId);
    Task<CourseEntity?> GetCourseByCodeAsync(string courseCode, string uniId);
    Task<CourseEntity?> GetCourseByIdAsync(int courseId, string uniId);
    Task<bool> IsCourseAccessibleToUser(CourseEntity courseEntity, string uniId);
    Task<bool> DoesCourseExistAsync(string courseCode);
    Task<bool> DoesCourseExistByIdAsync(int id);

}