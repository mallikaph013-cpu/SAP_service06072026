using System.Diagnostics;
using ITRepairService.Data;
using Microsoft.AspNetCore.Mvc;
using ITRepairService.Models;
using Microsoft.EntityFrameworkCore;

namespace ITRepairService.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _dbContext;

    public HomeController(ILogger<HomeController> logger, AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        var news = await _dbContext.NewsItems
            .Where(newsItem => newsItem.IsActive)
            .OrderByDescending(newsItem => newsItem.CreatedAt)
            .Take(6)
            .ToListAsync();

        return View(news);
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
