﻿using System.Data;
using App.Domain;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF;

public class AttendanceRepository(AppDbContext context)
{
    public async Task<bool> AddAttendanceCheck(AttendanceCheckEntity attendance, string creator, WorkplaceEntity? workplace)
    {
        attendance.CreatedBy = creator;
        attendance.UpdatedBy = creator;
        attendance.CreatedAt = DateTime.Now.ToUniversalTime();
        attendance.UpdatedAt= DateTime.Now.ToUniversalTime();

        if (workplace != null)
        {
            attendance.WorkplaceId = workplace.Id;
            attendance.Workplace = workplace;
        }
        
        await context.AttendanceChecks.AddAsync(attendance);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<CourseAttendanceEntity?> GetCurrentAttendance(int userId)
    {
        var ongoingAttendance= await context.CourseAttendances
            .Where(ca => ca.StartTime <= DateTime.Now && ca.EndTime >= DateTime.Now &&
                         ca.Course!.CourseTeacherEntities!.Any(ct => ct.TeacherId == userId)).
            Include(ca => ca.Course).Include(ca => ca.AttendanceType)
            .FirstOrDefaultAsync();
        
        return ongoingAttendance;
    }
    
    public async Task<bool> AddAttendance(CourseAttendanceEntity attendance)
    {
        var doesAttendanceExist = context.CourseAttendances.Any(ca => ca.CourseId == attendance.CourseId && 
                                                                      ca.StartTime == attendance.StartTime && 
                                                                      ca.EndTime == attendance.EndTime);

        if (doesAttendanceExist)
        {
            return false;
        }

        if (attendance.StartTime > attendance.EndTime)
        {
            return false;
        }   
        
        attendance.CreatedAt = DateTime.Now.ToUniversalTime();
        attendance.UpdatedAt = DateTime.Now.ToUniversalTime();
        
        await context.CourseAttendances.AddAsync(attendance);
       
        return await context.SaveChangesAsync() > 0 ;
    }
    
    public async Task<bool> UpdateAttendance(int attendanceId, CourseAttendanceEntity updatedAttendance)
    {
        var attendance = await context.CourseAttendances.FirstOrDefaultAsync(a => a.Id == attendanceId);
        if (attendance == null)
        {
            return false;
        }

        attendance.CourseId = updatedAttendance.CourseId;
        attendance.AttendanceTypeId = updatedAttendance.AttendanceTypeId;
        attendance.StartTime = updatedAttendance.StartTime;
        attendance.EndTime = updatedAttendance.EndTime;
        attendance.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> DeleteAttendanceEntity(CourseAttendanceEntity attendanceEntity)
    {
        context.CourseAttendances.Remove(attendanceEntity);
        return await context.SaveChangesAsync() > 0 ;
    }
    
    public async Task<bool> DeleteAttendanceCheckEntity(AttendanceCheckEntity attendanceCheckEntity)
    {
        context.AttendanceChecks.Remove(attendanceCheckEntity);
        return await context.SaveChangesAsync() > 0;
    }
    public async Task<int> GetStudentCountByAttendanceId(int attendanceId)
    {
        var attendanceCounts = await context.AttendanceChecks.Where(a => a.CourseAttendanceId == attendanceId)
            .CountAsync();
        return attendanceCounts;
    }

    public async Task<CourseAttendanceEntity?> GetAttendanceById(int attendanceId)
    {
        var attendance = await context.CourseAttendances
            .Include(u => u.AttendanceType)
            .Include(u => u.Course)
            .FirstOrDefaultAsync(u => u.Id == attendanceId);

        if (attendance != null)
        {
            attendance.StartTime = DateTime.SpecifyKind(attendance.StartTime, DateTimeKind.Utc);
            attendance.EndTime = DateTime.SpecifyKind(attendance.EndTime, DateTimeKind.Utc);
        }

        return attendance;
    }

    public async Task<bool> WorkplaceAvailabilityCheckById(int workplaceId)
    {
        return await context.Workplaces.AnyAsync(w => w.Id == workplaceId);
    }
    
    public async Task<WorkplaceEntity?> GetWorkplaceById(int workplaceId)
    {
        return await context.Workplaces.FirstOrDefaultAsync(w => w.Id == workplaceId);
    }
    
    public async Task<bool> AttendanceAvailabilityCheckById(int attendanceId)
    {
        return await context.CourseAttendances.AnyAsync(u => u.Id == attendanceId);
    }
    
    public async Task<bool> AttendanceCheckAvailabilityCheck(string studentCode, int attendanceId)
    {
        return  await context.AttendanceChecks.AnyAsync(u => u.StudentCode == studentCode 
                                                              && u.CourseAttendanceId == attendanceId);
    }
    
    public async Task<List<CourseAttendanceEntity>> GetCourseAttendancesByCourseId(int courseId)
    {
        var attendances = await context.CourseAttendances
            .Where(c => c.CourseId == courseId)
            .Include(c => c.Course)
            .ToListAsync();

        foreach (var attendance in attendances)
        {
            attendance.StartTime = DateTime.SpecifyKind(attendance.StartTime, DateTimeKind.Utc);
            attendance.EndTime = DateTime.SpecifyKind(attendance.EndTime, DateTimeKind.Utc);
        }

        return attendances;
    }

    public async Task<List<AttendanceCheckEntity>> GetAttendanceChecksByAttendanceId(int attendanceId)
    {
        return await context.AttendanceChecks
            .Where(c => c.CourseAttendanceId == attendanceId).ToListAsync();
    }
    
    public async Task<AttendanceCheckEntity?> GetAttendanceCheckById(int attendanceCheckId)
    {
        return await context.AttendanceChecks.FirstOrDefaultAsync(ca => ca.Id == attendanceCheckId);
    }
    
    
    public async Task<CourseAttendanceEntity?> GetMostRecentAttendanceByUser(int userId)
    {
        return await context.CourseAttendances
            .Where(ca => ca.Course!.CourseTeacherEntities!
                .Any(ct => ct.TeacherId == userId) && ca.StartTime <= DateTime.Now) 
            .Include(ca => ca.Course)
            .Include(ca => ca.AttendanceType) 
            .OrderByDescending(ca => ca.EndTime)
            .FirstOrDefaultAsync();
    }
    
    public async Task<List<AttendanceTypeEntity>> GetAttendanceTypes()
    {
        return await context.AttendanceTypes.ToListAsync();
    }
    
    public async Task<AttendanceTypeEntity?> GetAttendanceTypeById(int attendanceTypeId)
    {
        return await context.AttendanceTypes
            .FirstOrDefaultAsync(ca => ca.Id == attendanceTypeId);
    }
    
    public async Task<bool> RemoveOldAttendances(DateTime datePeriod)
    {
        var oldAttendances = await context.CourseAttendances
            .Where(u => u.EndTime < datePeriod)
            .ToListAsync();

        if (!oldAttendances.Any())
        {
            return false;
        }

        context.CourseAttendances.RemoveRange(oldAttendances);
        await context.SaveChangesAsync();
        
        return true;
    }
    
    public async Task<bool> RemoveOldAttendanceChecks(DateTime datePeriod)
    {
        var oldAttendanceChecks = await context.AttendanceChecks
            .Where(u => u.UpdatedAt < datePeriod && u.Deleted == true)
            .ToListAsync();

        if (!oldAttendanceChecks.Any())
        {
            return false;
        }

        context.AttendanceChecks.RemoveRange(oldAttendanceChecks);
        await context.SaveChangesAsync();
        
        return true;
    }
}