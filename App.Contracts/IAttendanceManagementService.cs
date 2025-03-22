using App.Domain;

namespace Contracts;

public interface IAttendanceManagementService
{
    Task<CourseAttendanceEntity?> GetCurrentAttendanceAsync(UserEntity user);
    Task<CourseAttendanceEntity?> GetCourseAttendanceByIdAsync(int attendanceId);
    Task AddAttendanceCheckAsync(AttendanceCheckEntity attendanceCheck, string creator);
}