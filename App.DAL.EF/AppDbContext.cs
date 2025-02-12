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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AttendanceCheckEntity>().ToTable("AttendanceChecks");
        modelBuilder.Entity<AttendanceTypeEntity>().ToTable("AttendanceTypes");
        modelBuilder.Entity<CourseAttendanceEntity>().ToTable("CourseAttendances");
        modelBuilder.Entity<CourseEntity>().ToTable("Courses");
        modelBuilder.Entity<CourseTeacherEntity>().ToTable("CourseTeachers");
        modelBuilder.Entity<UserEntity>().ToTable("Users");
        modelBuilder.Entity<UserTypeEntity>().ToTable("UserTypes");
        modelBuilder.Entity<WorkplaceEntity>().ToTable("Workplaces");
    }
}