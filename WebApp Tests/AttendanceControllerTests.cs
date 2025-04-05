using System.Security.Claims;
using App.Domain;
using Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework.Legacy;
using WebApp.ApiControllers;

namespace WebApp_Tests;

[TestFixture]
public class AttendanceControllerTests
{
    private Mock<IAttendanceManagementService> _attendanceServiceMock = null!;
    private Mock<ICourseManagementService> _courseServiceMock = null!;
    private Mock<IUserManagementService> _userServiceMock = null!;
    private Mock<ILogger<AttendanceController>> _loggerMock = null!;
    private AttendanceController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _attendanceServiceMock = new Mock<IAttendanceManagementService>();
        _courseServiceMock = new Mock<ICourseManagementService>();
        _userServiceMock = new Mock<IUserManagementService>();
        _loggerMock = new Mock<ILogger<AttendanceController>>();

        _controller = new AttendanceController(
            _attendanceServiceMock.Object,
            _courseServiceMock.Object,
            _userServiceMock.Object,
            _loggerMock.Object
        );

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.UserData, "test-uni-id"),
            new Claim(ClaimTypes.Role, "Teacher")
        };
        var identity = new ClaimsIdentity(claims, "mock");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = principal
            }
        };
    }

    [Test]
    public async Task GetAttendanceById_ReturnsOk_WhenAttendanceExists()
    {
        var attendance = new CourseAttendanceEntity { Id = 1 };
        _attendanceServiceMock
            .Setup(s => s.GetCourseAttendanceByIdAsync(1, "test-uni-id"))
            .ReturnsAsync(attendance);

        var result = await _controller.GetAttendanceById(1);

        Assert.That(result, Is.TypeOf<ActionResult<CourseAttendanceEntity>>());
        Assert.That(result.Value, Is.EqualTo(attendance));
    }

    [Test]
    public async Task GetAttendanceById_ReturnsNotFound_WhenAttendanceNotFound()
    {
        _attendanceServiceMock
            .Setup(s => s.GetCourseAttendanceByIdAsync(999, "test-uni-id"))
            .ReturnsAsync((CourseAttendanceEntity?)null);

        var result = await _controller.GetAttendanceById(999);

        Assert.That(result.Result, Is.TypeOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task GetCurrentAttendance_ReturnsOk_WhenUserFound()
    {
        var user = new UserEntity { Id = 1, UniId = "test-uni-id" };
        var attendance = new CourseAttendanceEntity { Id = 1, CourseId = 101 };

        _userServiceMock
            .Setup(s => s.GetUserByUniIdAsync("test-uni-id"))
            .ReturnsAsync(user);

        _attendanceServiceMock
            .Setup(s => s.GetCurrentAttendanceAsync(user.Id))
            .ReturnsAsync(attendance);

        var result = await _controller.GetCurrenAttendance("test-uni-id");

        Assert.That(result, Is.TypeOf<ActionResult<CourseAttendanceEntity>>());
        Assert.That(result.Value, Is.EqualTo(attendance));
    }

    [Test]
    public async Task GetCurrentAttendance_ReturnsNotFound_WhenUserNotFound()
    {
        _userServiceMock
            .Setup(s => s.GetUserByUniIdAsync("test-uni-id"))
            .ReturnsAsync((UserEntity?)null);

        var result = await _controller.GetCurrenAttendance("test-uni-id");

        Assert.That(result.Result, Is.TypeOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task GetCurrentAttendance_ReturnsNotFound_WhenAttendanceNotFound()
    {
        var user = new UserEntity { Id = 1, UniId = "test-uni-id" };

        _userServiceMock
            .Setup(s => s.GetUserByUniIdAsync("test-uni-id"))
            .ReturnsAsync(user);

        _attendanceServiceMock
            .Setup(s => s.GetCurrentAttendanceAsync(user.Id))
            .ReturnsAsync((CourseAttendanceEntity?)null);

        var result = await _controller.GetCurrenAttendance("test-uni-id");

        Assert.That(result.Result, Is.TypeOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task GetAttendanceStudentCount_ReturnsOk_WhenAttendanceExists()
    {
        var attendance = new CourseAttendanceEntity { Id = 1 };
        _attendanceServiceMock
            .Setup(s => s.GetCourseAttendanceByIdAsync(1, "test-uni-id"))
            .ReturnsAsync(attendance);

        _attendanceServiceMock
            .Setup(s => s.GetStudentsCountByAttendanceIdAsync(1))
            .ReturnsAsync(25);

        var result = await _controller.GetAttendanceStudentCount(1);

        Assert.That(result, Is.TypeOf<ActionResult<int>>());
        Assert.That(result.Value, Is.EqualTo(25));
    }

    [Test]
    public async Task GetAttendanceStudentCount_ReturnsNotFound_WhenAttendanceNotFound()
    {
        _attendanceServiceMock
            .Setup(s => s.GetCourseAttendanceByIdAsync(999, "test-uni-id"))
            .ReturnsAsync((CourseAttendanceEntity?)null);

        var result = await _controller.GetAttendanceStudentCount(999);

        Assert.That(result.Result, Is.TypeOf<NotFoundObjectResult>());
    }
    
    [Test]
    public async Task GetAttendanceById_ReturnsForbidden_WhenUserIsNotTeacher()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.UserData, "test-uni-id"),
            new Claim(ClaimTypes.Role, "Student") // Not "Teacher"
        };
        var identity = new ClaimsIdentity(claims, "mock");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = principal
            }
        };

        var result = await _controller.GetAttendanceById(1);

        Assert.That(result.Result, Is.TypeOf<ForbidResult>());
    }

    [Test]
    public async Task AddCourseAttendance_ReturnsForbidden_WhenUserIsNotTeacher()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.UserData, "test-uni-id"),
            new Claim(ClaimTypes.Role, "Student") // Not "Teacher"
        };
        var identity = new ClaimsIdentity(claims, "mock");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = principal
            }
        };

        var result = await _controller.AddCourseAttendance(new AttendanceModel());

        Assert.That(result.Result, Is.TypeOf<ForbidResult>());
    }
    

    [Test]
    public async Task AddAttendanceCheck_ReturnsOk_WhenValid()
    {
        var model = new AttendanceCheckModel
        {
            StudentCode = "12345",
            CourseAttendanceId = 1,
            Creator = "test-creator"
        };

        _attendanceServiceMock
            .Setup(s => s.AddAttendanceCheckAsync(It.IsAny<AttendanceCheckEntity>(), model.Creator, null))
            .ReturnsAsync(true);

        var result = await _controller.AddAttendanceCheck(model);

        Assert.That(result, Is.TypeOf<OkResult>());
    }

    [Test]
    public async Task AddAttendanceCheck_ReturnsBadRequest_WhenInvalid()
    {
        var model = new AttendanceCheckModel
        {
            StudentCode = "12345",
            CourseAttendanceId = 1,
            Creator = "test-creator"
        };

        _attendanceServiceMock
            .Setup(s => s.AddAttendanceCheckAsync(It.IsAny<AttendanceCheckEntity>(), model.Creator, null))
            .ReturnsAsync(false);

        var result = await _controller.AddAttendanceCheck(model);

        Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task AddAttendanceCheck_ReturnsBadRequest_WhenWorkplaceNotFound()
    {
        var model = new AttendanceCheckEntity()
        {
            StudentCode = "12345",
            CourseAttendanceId = 1,
            WorkplaceId = 999,
            CreatedBy = "test", 
            UpdatedBy = "test"
        };

        _attendanceServiceMock
            .Setup(s => s.DoesWorkplaceExist(999))
            .ReturnsAsync(false);

        var result = await _controller.AddAttendanceCheck(model);

        Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
    }

}