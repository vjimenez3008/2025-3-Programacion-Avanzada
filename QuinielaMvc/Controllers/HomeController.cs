using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyNewMvcApp.Models;

namespace MyNewMvcApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult About()
    {
        // Current time in format yyyy-MM-dd HH:mm:ss (24-hour)
        var now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        ViewData["Now"] = now;
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
