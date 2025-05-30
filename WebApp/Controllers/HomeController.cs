using System.Diagnostics;
using App.DAL.EF;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

public class AdminPanelController(IAdminAccessService adminAccessService, ILogger<AdminPanelController> logger)
    : BaseController(adminAccessService)
{
    private readonly IAdminAccessService _adminAccessService = adminAccessService;

    [HttpGet]
    public IActionResult Index(string? message)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        var model = new AdminLoginModel
        {
            Username = string.Empty,
            Password = string.Empty,
            Message = message ?? string.Empty
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index([Bind("Username", "Password")] AdminLoginModel model)
    {
        logger.LogInformation($"{HttpContext.Request.Method.ToUpper()} - {HttpContext.Request.Path}");
        if (!_adminAccessService.AdminAccessGrant(model.Username, model.Password))
        {
            return Index("Wrong username or password!");
        }
        
        await SetTokensAsync();
        logger.LogInformation($"Admin access granted successfully");
        return  RedirectToAction("Home");
    }

    public IActionResult LogOut()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public async Task<IActionResult> Home(string? message)
    {
        var tokenValidity = await IsTokenValidAsync(HttpContext);
        if (!tokenValidity)
        {
            return Unauthorized("You cannot access admin panel without logging in!");
        }
        
        var model = new AdminLoginModel
        {
            Username = string.Empty,
            Password = string.Empty,
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