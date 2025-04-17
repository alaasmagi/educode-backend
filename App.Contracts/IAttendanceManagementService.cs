using App.Domain;

namespace Contracts;

public interface IAttendanceManagementService
{
    Task<CourseAttendanceEntity?> GetCurrentAttendanceAsync(int userId);
    Task<CourseAttendanceEntity?> GetCourseAttendanceByIdAsync(int attendanceId, string uniId);
    Task<bool> AddAttendanceCheckAsync(AttendanceCheckEntity attendanceCheck, string creator, int? workplaceId);
    Task<bool> DoesAttendanceCheckExist(string studentCode, string fullName, int attendanceId);
    Task<bool> DoesWorkplaceExist(int id);
    Task<List<CourseAttendanceEntity>?> GetAttendancesByCourseAsync(int courseId);
    Task<List<AttendanceCheckEntity>?> GetAttendanceChecksByAttendanceIdAsync(int attendanceId);
    Task<int> GetStudentsCountByAttendanceIdAsync(int attendanceId);
    Task<CourseAttendanceEntity?> GetMostRecentAttendanceByUserAsync(int userId);
    Task<List<AttendanceTypeEntity>?> GetAttendanceTypesAsync();
    Task<AttendanceTypeEntity?> GetAttendanceTypeByIdAsync(int attendanceTypeId);
    Task<bool> AddAttendanceAsync(CourseAttendanceEntity newAttendance, List<DateOnly> attendanceDates,
        TimeOnly startTime, TimeOnly endTime);

    Task<AttendanceCheckEntity?> GetAttendanceCheckByIdAsync(int id, string uniId);
    Task<bool> DeleteAttendance(int id, string uniId);
    Task<bool> EditAttendanceAsync(int attendanceId, CourseAttendanceEntity updatedAttendance);
    Task<bool> DeleteAttendanceCheck(int id, string uniId);
    Task<bool> IsAttendanceAccessibleByUser(CourseAttendanceEntity attendance, string uniId);
    Task<bool> IsAttendanceCheckAccessibleByUser(AttendanceCheckEntity attendanceCheck, string uniId);
}