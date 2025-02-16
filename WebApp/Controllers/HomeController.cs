using System.Diagnostics;
using App.BLL;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

public class AdminPanelController(ILogger<AdminPanelController> logger) : Controller
{
    private readonly ILogger<AdminPanelController> _logger = logger;
    private AccessManagement access = new();

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
        return !access.AdminAccessGrant(model.Username, model.Password) ? Index("Wrong username or password!") : RedirectToAction("Home");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Home(string? message)
    {
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