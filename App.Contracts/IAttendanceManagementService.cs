using App.Domain;

namespace Contracts;

public interface IAttendanceManagementService
{
    Task<CourseAttendanceEntity?> GetCurrentAttendanceAsync(int userId);
    Task<CourseAttendanceEntity?> GetCourseAttendanceByIdAsync(int attendanceId);
    Task AddAttendanceCheckAsync(AttendanceCheckEntity attendanceCheck, string creator);
    Task<List<CourseAttendanceEntity>?> GetAttendancesByCourseAsync(int courseId);
    Task<List<AttendanceCheckEntity>?> GetAttendanceChecksByAttendanceIdAsync(int attendanceId);
    Task<CourseAttendanceEntity?> GetMostRecentAttendanceByUserAsync(int userId);
}