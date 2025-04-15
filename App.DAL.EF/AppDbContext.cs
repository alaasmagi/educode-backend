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
    public DbSet<UserAuthEntity> UserAuthData { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // UserEntity relationship
        modelBuilder.Entity<UserEntity>()
            .ToTable("Users")
            .HasOne(u => u.UserType)
            .WithMany()
            .HasForeignKey(u => u.UserTypeId);
        modelBuilder.Entity<UserEntity>()
            .HasIndex(u => u.UniId)
            .IsUnique();
        modelBuilder.Entity<UserEntity>()
            .HasIndex(u => u.StudentCode)
            .IsUnique();
        modelBuilder.Entity<UserEntity>()
            .HasIndex(u => u.FullName)
            .IsUnique();
        
        // UserAuthToken relationship
        modelBuilder.Entity<UserAuthEntity>()
            .ToTable("UserAuth")
            .HasOne(u => u.User)
            .WithOne()
            .HasForeignKey<UserAuthEntity>(u => u.UserId);
        modelBuilder.Entity<UserAuthEntity>()
            .HasIndex(u => u.UserId)
            .IsUnique();
        
        // CourseAttendance relationship
        modelBuilder.Entity<CourseAttendanceEntity>()
            .ToTable("CourseAttendances")
            .HasMany(u => u.AttendanceChecks)
            .WithOne()
            .HasForeignKey(u => u.CourseAttendanceId);
        modelBuilder.Entity<CourseAttendanceEntity>()
            .HasOne(u => u.Course)
            .WithMany()
            .HasForeignKey(u => u.CourseId);
        modelBuilder.Entity<CourseAttendanceEntity>()
            .HasOne(u => u.AttendanceType)
            .WithMany()
            .HasForeignKey(u => u.AttendanceTypeId);
        
        // AttendanceCheck relationship
        modelBuilder.Entity<AttendanceCheckEntity>()
            .ToTable("AttendanceChecks");
         modelBuilder.Entity<AttendanceCheckEntity>()
                .HasIndex(e => new { e.StudentCode, e.FullName, e.CourseAttendanceId })
                .IsUnique();
        modelBuilder.Entity<AttendanceCheckEntity>()
            .HasOne(u => u.Workplace)
            .WithMany()
            .HasForeignKey(u => u.WorkplaceId);
        
        // Course relationship
        modelBuilder.Entity<CourseEntity>()
            .ToTable("Courses");
        modelBuilder.Entity<CourseEntity>()
            .HasMany(u => u.CourseTeacherEntities)
            .WithOne(u => u.Course)
            .HasForeignKey(u => u.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<CourseEntity>()
            .HasIndex(u => u.CourseCode)
            .IsUnique();
        
        // CourseTeacher relationship
        modelBuilder.Entity<CourseTeacherEntity>()
            .ToTable("CourseTeachers")
            .HasOne(u => u.Course)
            .WithMany(u => u.CourseTeacherEntities)
            .HasForeignKey(u => u.CourseId);
        modelBuilder.Entity<CourseTeacherEntity>()
            .HasOne(u => u.Teacher)
            .WithMany()
            .HasForeignKey(u => u.TeacherId);
        
        // UserType relationship
        modelBuilder.Entity<UserTypeEntity>()
            .ToTable("UserTypes");
        modelBuilder.Entity<UserTypeEntity>()
            .HasIndex(u => u.UserType)
            .IsUnique();
        
        // AttendanceType relationship
        modelBuilder.Entity<AttendanceTypeEntity>()
            .ToTable("AttendanceTypes");
        modelBuilder.Entity<AttendanceTypeEntity>()
            .HasIndex(u => u.AttendanceType)
            .IsUnique();
        
        // Workplace relationship
        modelBuilder.Entity<WorkplaceEntity>()
            .ToTable("Workplaces");
        
    }
}
