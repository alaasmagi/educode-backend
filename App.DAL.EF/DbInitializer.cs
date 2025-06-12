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
                    Id = Guid.NewGuid(),
                    UserType = "student",
                    CreatedBy = "aspnet-initializer",
                    CreatedAt = now,
                    UpdatedBy = "aspnet-initializer",
                    UpdatedAt = now,
                    Deleted = false
                },
                new UserTypeEntity
                {
                    Id = Guid.NewGuid(),
                    UserType = "teacher",
                    CreatedBy = "aspnet-initializer",
                    CreatedAt = now,
                    UpdatedBy = "aspnet-initializer",
                    UpdatedAt = now,
                    Deleted = false
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
                    Id = Guid.NewGuid(),
                    CourseStatus = "available",
                    CreatedBy = "aspnet-initializer",
                    CreatedAt = now,
                    UpdatedBy = "aspnet-initializer",
                    UpdatedAt = now,
                    Deleted = false
                },
                new CourseStatusEntity
                {
                    Id = Guid.NewGuid(),
                    CourseStatus = "unavailable",
                    CreatedBy = "aspnet-initializer",
                    CreatedAt = now,
                    UpdatedBy = "aspnet-initializer",
                    UpdatedAt = now,
                    Deleted = false
                },
                new CourseStatusEntity
                {
                    Id = Guid.NewGuid(),
                    CourseStatus = "temp-unavailable",
                    CreatedBy = "aspnet-initializer",
                    CreatedAt = now,
                    UpdatedBy = "aspnet-initializer",
                    UpdatedAt = now,
                    Deleted = false
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
                    Id = Guid.NewGuid(),
                    AttendanceType = "lecture",
                    CreatedBy = "aspnet-initializer",
                    CreatedAt = now,
                    UpdatedBy = "aspnet-initializer",
                    UpdatedAt = now,
                    Deleted = false
                },
                new AttendanceTypeEntity
                {
                    Id = Guid.NewGuid(),
                    AttendanceType = "practice",
                    CreatedBy = "aspnet-initializer",
                    CreatedAt = now,
                    UpdatedBy = "aspnet-initializer",
                    UpdatedAt = now,
                    Deleted = false
                },
                new AttendanceTypeEntity
                {
                    Id = Guid.NewGuid(),
                    AttendanceType = "lecture-practice",
                    CreatedBy = "aspnet-initializer",
                    CreatedAt = now,
                    UpdatedBy = "aspnet-initializer",
                    UpdatedAt = now,
                    Deleted = false
                }
            };

            context.AttendanceTypes.AddRange(attendanceTypes);
            context.SaveChanges();
        }
    }
    
}