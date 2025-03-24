using App.Domain;

namespace Contracts;

public interface IAttendanceManagementService
{
    Task<CourseAttendanceEntity?> GetCurrentAttendanceAsync(int userId);
    Task<CourseAttendanceEntity?> GetCourseAttendanceByIdAsync(int attendanceId);
    Task<bool> AddAttendanceCheckAsync(AttendanceCheckEntity attendanceCheck, string creator, int? workplaceId);
    Task<bool> DoesAttendanceCheckExist(string studentCode, int attendanceId);
    Task<bool> DoesWorkplaceExist(int id);
    Task<List<CourseAttendanceEntity>?> GetAttendancesByCourseAsync(int courseId);
    Task<List<AttendanceCheckEntity>?> GetAttendanceChecksByAttendanceIdAsync(int attendanceId);
    int GetStudentsCountByAttendanceIdAsync(CourseAttendanceEntity attendanceEntity);
    Task<CourseAttendanceEntity?> GetMostRecentAttendanceByUserAsync(int userId);
    Task<List<AttendanceTypeEntity>?> GetAttendanceTypesAsync();
    Task<AttendanceTypeEntity?> GetAttendanceTypeByIdAsync(int attendanceTypeId);
    Task<bool> AddAttendanceAsync(CourseAttendanceEntity newAttendance, List<DateOnly> attendanceDates,
        TimeOnly startTime, TimeOnly endTime);
    Task<AttendanceCheckEntity?> GetAttendanceCheckByIdAsync(int id);
    Task<bool> DeleteAttendance(int id);
    Task<bool> EditAttendanceAsync(int attendanceId, CourseAttendanceEntity updatedAttendance);
    Task<bool> DeleteAttendanceCheck(int id);

}