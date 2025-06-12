using App.Domain;

namespace App.DAL.EF;

public class DbInitializer
{
    public static void SeedUserTypes(AppDbContext context)
    {
        if (!context.UserTypes.Any())
        {
            var now = DateTime.Now.ToUniversalTime();

            var userTypes = new List<UserTypeEntity>
            {
                new UserTypeEntity
                {
                    UserType = "student",
                    CreatedBy = "aspnet-initializer",
                    CreatedAt = now,
                    UpdatedBy = "aspnet-initializer",
                    UpdatedAt = now,
                },
                new UserTypeEntity
                {
                    UserType = "teacher",
                    CreatedBy = "aspnet-initializer",
                    CreatedAt = now,
                    UpdatedBy = "aspnet-initializer",
                    UpdatedAt = now,
                }
            };
            
            context.UserTypes.AddRange(userTypes);
            context.SaveChanges();
        }
    }
    
    public static void SeedCourseStatuses(AppDbContext context)
    {
        if (!context.CourseStatuses.Any())
        {
            var now = DateTime.Now.ToUniversalTime();

            var courseStatuses = new List<CourseStatusEntity>
            {
                new CourseStatusEntity
                {
                    CourseStatus = "available",
                    CreatedBy = "aspnet-initializer",
                    CreatedAt = now,
                    UpdatedBy = "aspnet-initializer",
                    UpdatedAt = now,
                },
                new CourseStatusEntity
                {
                    CourseStatus = "unavailable",
                    CreatedBy = "aspnet-initializer",
                    CreatedAt = now,
                    UpdatedBy = "aspnet-initializer",
                    UpdatedAt = now,
                },
                new CourseStatusEntity
                {
                    CourseStatus = "temp-unavailable",
                    CreatedBy = "aspnet-initializer",
                    CreatedAt = now,
                    UpdatedBy = "aspnet-initializer",
                    UpdatedAt = now,
                }
            };

            context.CourseStatuses.AddRange(courseStatuses);
            context.SaveChanges();
        }
    }
    
    public static void SeedAttendanceTypes(AppDbContext context)
    {
        if (!context.AttendanceTypes.Any())
        {
            var now = DateTime.Now.ToUniversalTime();

            var attendanceTypes = new List<AttendanceTypeEntity>
            {
                new AttendanceTypeEntity
                {
                    AttendanceType = "lecture",
                    CreatedBy = "aspnet-initializer",
                    CreatedAt = now,
                    UpdatedBy = "aspnet-initializer",
                    UpdatedAt = now,
                },
                new AttendanceTypeEntity
                {
                    AttendanceType = "practice",
                    CreatedBy = "aspnet-initializer",
                    CreatedAt = now,
                    UpdatedBy = "aspnet-initializer",
                    UpdatedAt = now,
                },
                new AttendanceTypeEntity
                {
                    AttendanceType = "lecture-practice",
                    CreatedBy = "aspnet-initializer",
                    CreatedAt = now,
                    UpdatedBy = "aspnet-initializer",
                    UpdatedAt = now,
                }
            };

            context.AttendanceTypes.AddRange(attendanceTypes);
            context.SaveChanges();
        }
    }
    
}