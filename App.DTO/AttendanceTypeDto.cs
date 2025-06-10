using App.Domain;

namespace App.DTO;

public class AttendanceTypeDto(AttendanceTypeEntity attendanceType)
{
    public Guid Id { get; set; } = attendanceType.Id;
    public string AttendanceType { get; set; } = attendanceType.AttendanceType;
}