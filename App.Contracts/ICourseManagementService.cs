using App.Domain;
using App.DTO;

namespace Contracts;

public interface ICourseManagementService
{
    Task<CourseEntity?> GetCourseByAttendanceIdAsync(Guid attendanceId);
    Task<bool> AddCourse(UserEntity user, CourseEntity course, string creator);
    Task<bool> EditCourse(Guid courseId, CourseEntity newCourse);
    Task<bool> DeleteCourse(Guid courseId, string uniId);
    Task<List<CourseStatusEntity>?> GetAllCourseStatuses();
    Task<List<CourseEntity>?> GetCoursesByUserAsync(Guid userId);
    Task<List<AttendanceStudentCountDto>?> GetAttendancesUserCountsByCourseAsync(Guid courseId);
    Task<CourseEntity?> GetCourseByNameAsync(string courseName, string uniId);
    Task<CourseEntity?> GetCourseByCodeAsync(string courseCode, string uniId);
    Task<CourseEntity?> GetCourseByIdAsync(Guid courseId, string uniId);
    Task<bool> IsCourseAccessibleToUser(CourseEntity courseEntity, string uniId);
    Task<bool> DoesCourseExistAsync(string courseCode);
    Task<bool> DoesCourseExistByIdAsync(Guid id);

}