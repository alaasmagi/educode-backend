using App.Domain;

namespace Contracts;

public interface IAttendanceManagementService
{
    Task<CourseAttendanceEntity?> GetCurrentAttendanceAsync(Guid userId);
    Task<CourseAttendanceEntity?> GetCourseAttendanceByIdAsync(Guid attendanceId, string uniId);
    Task<bool> AddAttendanceCheckAsync(AttendanceCheckEntity attendanceCheck, string creator, int? workplaceIdentifier);
    Task<bool> DoesAttendanceCheckExist(string studentCode, string fullName, int attendanceIdentifier);
    Task<bool> DoesWorkplaceExist(int workplaceIdentifier);
    Task<List<CourseAttendanceEntity>?> GetAttendancesByCourseAsync(Guid courseId);
    Task<List<AttendanceCheckEntity>?> GetAttendanceChecksByAttendanceIdAsync(int attendanceIdentifier);
    Task<int> GetStudentsCountByAttendanceIdAsync(int attendanceIdentifier);
    Task<CourseAttendanceEntity?> GetMostRecentAttendanceByUserAsync(Guid userId);
    Task<List<AttendanceTypeEntity>?> GetAttendanceTypesAsync();
    Task<AttendanceTypeEntity?> GetAttendanceTypeByIdAsync(Guid attendanceTypeId);
    Task<bool> AddAttendanceAsync(CourseAttendanceEntity newAttendance, List<DateOnly> attendanceDates,
        TimeOnly startTime, TimeOnly endTime);

    Task<AttendanceCheckEntity?> GetAttendanceCheckByIdAsync(Guid id, string uniId);
    Task<bool> DeleteAttendance(Guid attendanceId, string uniId);
    Task<bool> EditAttendanceAsync(Guid attendanceId, CourseAttendanceEntity updatedAttendance);
    Task<bool> DeleteAttendanceCheck(Guid attendanceCheckId, string uniId);
    Task<bool> IsAttendanceAccessibleByUser(CourseAttendanceEntity attendance, string uniId);
    Task<bool> IsAttendanceCheckAccessibleByUser(AttendanceCheckEntity attendanceCheck, string uniId);
}