using System.Diagnostics;
using App.BLL;
using App.DAL.EF;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

public class AdminPanelController : BaseController
{
    private readonly ILogger<AdminPanelController> _logger;
    private readonly AdminAccessManagement _access;
    private readonly AuthBrain _auth;
    public AdminPanelController(
        ILogger<AdminPanelController> logger,
        AppDbContext context,
        IConfiguration configuration)
    {
        _logger = logger;
        _auth = new AuthBrain(configuration);
        _access = new AdminAccessManagement();
    }

    [HttpGet]
    public IActionResult Index(string? message)
    {
        var model = new LoginModel
        {
            Message = message ?? string.Empty
        };

        return View(model);
    }

    [HttpPost]
    public IActionResult Index([Bind("Username", "Password")] LoginModel model)
    {
        if (!_access.AdminAccessGrant(model.Username, model.Password))
        {
            return Index("Wrong username or password!");
        }
        
        SetTokens();
        return  RedirectToAction("Home");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Home(string? message)
    {
        if (!IsTokenValid(HttpContext))
        {
            return Unauthorized("You cannot access admin panel without logging in!");
        }
        
        var model = new LoginModel
        {
            Message = message ?? string.Empty
        };
        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}