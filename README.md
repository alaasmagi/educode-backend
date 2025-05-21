# educode-backend
## Description

* UI language (Admin UI): English
* Development year: **2025**
* Languages and technologies: **C#, .NET Core, ASP.NET MVC, JWT, Entity Framework Core**
* This is the backend component of my Bachelor's final thesis project, which also includes [mobile app](https://github.com/alaasmagi/educode-mobile) and [browser client](https://github.com/alaasmagi/educode-web)
* Detailed documentation of my Bachelor's final thesis:<link>.

## How to run - option #1

### Prerequisites

* .NET SDK 9.0

The application should have .env file in the root folder `/` and it shoult have following content:
```bash
HOST=<your-db-host>
PORT=<your-db-port>
USER=<your-db-username>
DB=<your-db-name>
DBKEY=<your-db-password>

OTPKEY=<hash-for-otp-generation> //OTP-s are used to create accounts and recover forgotten passwords

ADMINUSER=<your-bcrypted(12rounds)-and-base64-encoded-admin-username>
ADMINKEY=<your-bcrypted(12rounds)-and-base64-encoded-admin-password>
ADMINTOKENSALT=<your-admin-token-salt-for-token-generation-and-verification> //Use some kind of hash value

JWTKEY=<your-json-web-token-authenticity-key> //Use some kind of hashed value
JWTAUD=<your-json-web-token-audience>
JWTISS=<your-json-web-token-issuer>

MAILSENDER_EMAIL=<external-smtp-mailservice-email-address>
MAILSENDER_KEY=<external-smtp-mailservice-key>
MAILSENDER_HOST=<external-smtp-mailsender-host>
MAILSENDER_PORT=<external-smtp-port>

EMAILDOMAIN=<email-domain-for-otp> //For example: "@taltech.ee"

FRONTENDURL=<web-frontend-url-for-cors>
```
The idea behind this complicated .env file is that if government decides to change something about taxation, the app does not need to be changed, only environment variables change.

### Running the app

After meeting all prerequisites above - 
* application can be run via terminal/cmd opened in the root of WebApp folder `/WebApp` by command
```bash
dotnet run
```
* user interface can be viewed from the web browser on the address the application provided in the terminal/cmd

## How to run #2

### Prerequisites

* .NET SDK 9.0

The application should have .env file in the root folder `/` and it shoult have following content:
```bash
HOST=<your-db-host>
PORT=<your-db-port>
USER=<your-db-username>
DB=<your-db-name>
DBKEY=<your-db-password>

OTPKEY=<hash-for-otp-generation> //OTP-s are used to create accounts and recover forgotten passwords

ADMINUSER=<your-bcrypted(12rounds)-and-base64-encoded-admin-username>
ADMINKEY=<your-bcrypted(12rounds)-and-base64-encoded-admin-password>
ADMINTOKENSALT=<your-admin-token-salt-for-token-generation-and-verification> //Use some kind of hash value

JWTKEY=<your-json-web-token-authenticity-key> //Use some kind of hashed value
JWTAUD=<your-json-web-token-audience>
JWTISS=<your-json-web-token-issuer>

MAILSENDER_EMAIL=<external-smtp-mailservice-email-address>
MAILSENDER_KEY=<external-smtp-mailservice-key>
MAILSENDER_HOST=<external-smtp-mailsender-host>
MAILSENDER_PORT=<external-smtp-port>

EMAILDOMAIN=<email-domain-for-otp> //For example: "@taltech.ee"

FRONTENDURL=<web-frontend-url-for-cors>
```

### Running the app

After meeting all prerequisites above - 
* application can be run via terminal/cmd opened in the root of WebApp folder `/WebApp` by command
```bash
dotnet run
```
* The Admin UI can be viewed from the web browser on the address the application provided in the terminal/cmd

## Features
* The Admin UI allows convenient management of the application's database.



## Design choices

### Application overall design
I used ASP.NET MVC, because I think, that keeping logic and view separate keeps the code clean, well structured and provides better testability. 

### Services
There are 7 main services:
* **AdminAccessService** - controls admin access to the Admin UI
* **AttendanceManagementService** - handles CRUD operations for attendances and attendance checks
* **AuthService** - responsible for JWT generation
* **CourseManagementService** - manages CRUD operations for courses
* **EmailService** - sends emails containing OTPs
* **OtpService** - handles OTP generation and verification
* **UserManagementService** - manages all CRUD operations related to users  

Additionally, there is a helper service:  
* **CleanupService** - performs automatic cleanup of attendances older than 6 months
  
### Database entities
There are 9 DB entities to manage user data, course data, attendance data and attendance check data.  
* **AttendanceCheckEntity**
```csharp
public class AttendanceCheckEntity : BaseEntity
{
    [Required]
    public string StudentCode { get; set; } = default!;
    [Required]
    public string FullName { get; set; } = default!;
    [Required]
    public int CourseAttendanceId { get; set; }
    public int? WorkplaceId { get; set; }
    public WorkplaceEntity? Workplace { get; set; }
}
```
* **AttendanceTypeEntity**
```csharp
public class AttendanceTypeEntity : BaseEntity
{
    [Required]
    [MaxLength(128)]
    public string AttendanceType { get; set; } = default!;
}
```
* **CourseAttendanceEntity**
```csharp
public class CourseAttendanceEntity : BaseEntity
{
    [Required]
    [ForeignKey("Course")]
    public int CourseId { get; set; }
    public CourseEntity? Course { get; set; }
    [Required]
    [ForeignKey("AttendanceType")]
    public int AttendanceTypeId { get; set; }
    public AttendanceTypeEntity? AttendanceType { get; set; }
    [Required]
    public DateTime StartTime { get; set; }
    [Required]
    public DateTime EndTime { get; set; }

    public ICollection<AttendanceCheckEntity>? AttendanceChecks { get; set; }
}
```
* **CourseEntity**
```csharp
public class CourseEntity : BaseEntity
{
    [Required]
    [MaxLength(128)]
    public string CourseCode { get; set; } = default!;
    [Required]
    [MaxLength(128)]
    public string CourseName { get; set; } = default!;
    [Required]
    public ECourseValidStatus CourseValidStatus { get; set; }
    public ICollection<CourseTeacherEntity>? CourseTeacherEntities { get; set; }
}
```
* **CourseTeacherEntity**
```csharp
public class CourseTeacherEntity : BaseEntity
{
    [Required]
    [ForeignKey("Course")]
    public int CourseId { get; set; }
    public CourseEntity? Course { get; set; }
    [Required]
    [ForeignKey("Teacher")]
    public int TeacherId { get; set; }
    public UserEntity? Teacher { get; set; }
}
```
* **UserAuthEntity**
```csharp
public class UserAuthEntity : BaseEntity
{
    [Required]
    [ForeignKey("User")]
    public int UserId { get; set; }
    public UserEntity? User { get; set; }
    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = default!;
}
```
* **UserEntity**
```csharp
public class UserEntity : BaseEntity
{
    [Required]
    [ForeignKey("UserType")]
    public int? UserTypeId { get; set; }
    public UserTypeEntity? UserType { get; set; }
    [Required]
    [MaxLength(128)]
    public string UniId { get; set; } = default!;
    [MaxLength(128)]
    public string? StudentCode { get; set; }
    [Required]
    [MaxLength(255)]
    public string FullName { get; set; } = default!;
}
```
* **UserTypeEntity**
```csharp
public class UserTypeEntity : BaseEntity
{
    [Required]
    [MaxLength(128)]
    public string UserType { get; set; } = default!;
    
}
```
* **WorkplaceEntity**
```csharp
public class WorkplaceEntity : BaseEntity
{
    [Required]
    [MaxLength(128)]
    public string ClassRoom { get; set; } = default!;
    [Required]
    [MaxLength(128)]
    public string ComputerCode { get; set; } = default!;
}
```

### BaseEntity
The `BaseEntity` class is defined in this project, and it is uploaded as a NuGet package [AL_AppDev.Base(v1.0.2)](https://www.nuget.org/packages/AL_AppDev.Base/1.0.2)
```csharp
public class BaseEntity
{
    [Required]
    public int Id { get; set; }
    [Required]
    [MaxLength(128)]
    public string CreatedBy { get; set; } = default!;
    [Required]
    public DateTime CreatedAt { get; set; }
    [Required]
    [MaxLength(128)]
    public string UpdatedBy { get; set; } = default!;
    [Required]
    public DateTime UpdatedAt { get; set; }
    [Required] 
    public bool Deleted { get; set; } = false;
}
```

### DTOs and enums
There are several DTOs and enums that are used in the application.  
* **CourseStatusDto**
```csharp
public class CourseStatusDto
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
}
```
* **CourseUserCountDto**
```csharp
public class CourseUserCountDto
{
    public DateTime AttendanceDate { get; set; }
    public int UserCount { get; set; } = 0;
}
```
* **ECourseValidStatus**
```csharp
public enum ECourseValidStatus
{
    Available,
    TempUnavailable,
    Unavailable
}
```
  
### User Interface (Admin UI)
* The Admin UI is implemented using ASP.NET MVC default pages (Views)
* Bootstrap is used for quick customisation

### Unit tests
* Unit tests cover 100% of the business logic
* Tests are written using the NUnit framework

## Improvements & scaling possibilities

### Integration with more education related services
* User testing results raised an idea of integrating this application with already existing infrastructure of the University (TalTech app)
