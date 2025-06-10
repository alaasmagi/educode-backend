using App.Domain;

namespace App.DTO;

public class AttendanceCheckDto(AttendanceCheckEntity attendanceCheck)
{
    public Guid Id { get; set; } = attendanceCheck.Id;
    public string StudentCode { get; set; } = attendanceCheck.StudentCode;
    public string FullName { get; set; } = attendanceCheck.FullName;
    public int AttendanceIdentifier { get; set; } = attendanceCheck.AttendanceIdentifier;
    public int? WorkplaceIdentifier { get; set; } = attendanceCheck.WorkplaceIdentifier;
}

