using App.Domain;
using Microsoft.EntityFrameworkCore;
namespace App.DAL.EF;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<AttendanceCheckEntity> AttendanceChecks { get; set; }
    public DbSet<AttendanceTypeEntity> AttendanceTypes { get; set; }
    public DbSet<CourseAttendanceEntity> CourseAttendances { get; set; }
    public DbSet<CourseEntity> Courses { get; set; }
    public DbSet<CourseTeacherEntity> CourseTeachers { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<UserTypeEntity> UserTypes { get; set; }
    public DbSet<WorkplaceEntity> Workplaces { get; set; }
    public DbSet<UserAuthTokenEntity> UserAuthTokens { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // UserEntity relationship
        modelBuilder.Entity<UserEntity>()
            .ToTable("Users")
            .HasOne(u => u.UserType)
            .WithMany(u => u.Users)
            .HasForeignKey(u => u.UserTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserEntity>()
            .HasMany(u => u.UserAuthTokens)
            .WithOne(u => u.User)
            .HasForeignKey(u => u.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<UserEntity>()
            .HasMany(u => u.AttendanceChecks)
            .WithOne(u => u.Student)
            .HasForeignKey(u => u.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<UserEntity>()
            .HasMany(u => u.CourseTeachers)
            .WithOne(u => u.Teacher)
            .HasForeignKey(u => u.TeacherId)
            .OnDelete(DeleteBehavior.Cascade);
        
        
        // UserAuthToken relationship
        modelBuilder.Entity<UserAuthTokenEntity>()
            .ToTable("UserAuthTokens")
            .HasOne(u => u.User)
            .WithMany(u => u.UserAuthTokens)
            .HasForeignKey(u => u.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        
        // CourseAttendance relationship
        modelBuilder.Entity<CourseAttendanceEntity>()
            .ToTable("CourseAttendances")
            .HasMany(u => u.AttendanceChecks)
            .WithOne(u => u.CourseAttendance)
            .HasForeignKey(u => u.CourseAttendanceId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<CourseAttendanceEntity>()
            .HasOne(u => u.Course)
            .WithMany(u => u.CourseAttendanceEntities)
            .HasForeignKey(u => u.CourseId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<CourseAttendanceEntity>()
            .HasOne(u => u.AttendanceType)
            .WithMany(u => u.CourseAttendanceEntities)
            .HasForeignKey(u => u.AttendanceTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        
        // AttendanceCheck relationship
        modelBuilder.Entity<AttendanceCheckEntity>()
            .ToTable("AttendanceChecks")
            .HasOne(u => u.Student)
            .WithMany(u => u.AttendanceChecks)
            .HasForeignKey(u => u.StudentId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<AttendanceCheckEntity>()
            .HasOne(u => u.CourseAttendance)
            .WithMany(u => u.AttendanceChecks)
            .HasForeignKey(u => u.CourseAttendanceId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<AttendanceCheckEntity>()
            .HasOne(u => u.Workplace)
            .WithMany(u => u.AttendanceChecks)
            .HasForeignKey(u => u.WorkplaceId)
            .OnDelete(DeleteBehavior.Restrict);

        
        // Course relationship
        modelBuilder.Entity<CourseEntity>()
            .ToTable("Courses")
            .HasMany(u=> u.CourseAttendanceEntities)
            .WithOne(u => u.Course)
            .HasForeignKey(u => u.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<CourseEntity>()
            .HasMany(u => u.CourseTeacherEntities)
            .WithOne(u => u.Course)
            .HasForeignKey(u => u.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
        
        
        // CourseTeacher relationship
        modelBuilder.Entity<CourseTeacherEntity>()
            .ToTable("CourseTeachers")
            .HasOne(u => u.Course)
            .WithMany(u => u.CourseTeacherEntities)
            .HasForeignKey(u => u.CourseId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<CourseTeacherEntity>()
            .HasOne(u => u.Teacher)
            .WithMany(u => u.CourseTeachers)
            .HasForeignKey(u => u.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);
        
        
        // UserType relationship
        modelBuilder.Entity<UserTypeEntity>()
            .ToTable("UserTypes")
            .HasMany(u => u.Users)
            .WithOne(u => u.UserType)
            .HasForeignKey(u => u.UserTypeId)
            .OnDelete(DeleteBehavior.SetNull);

        
        // AttendanceType relationship
        modelBuilder.Entity<AttendanceTypeEntity>()
            .ToTable("AttendanceTypes")
            .HasMany(u => u.CourseAttendanceEntities)
            .WithOne(u => u.AttendanceType)
            .HasForeignKey(u => u.AttendanceTypeId)
            .OnDelete(DeleteBehavior.Cascade);
        
        
        // Workplace relationship
        modelBuilder.Entity<WorkplaceEntity>()
            .ToTable("Workplaces")
            .HasMany(u => u.AttendanceChecks)
            .WithOne(u => u.Workplace)
            .HasForeignKey(u => u.WorkplaceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}