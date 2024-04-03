using AllupProject.DAL;
using AllupProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AllupProject.Controllers;

public class HomeController : Controller
{
    private readonly AllupDbContext _context;

    public HomeController(AllupDbContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        HomeViewModel homeViewModel = new HomeViewModel()
        {
            Sliders = await _context.Sliders.ToListAsync(),
            Banners = await _context.Banners.ToListAsync()
        };
        return View(homeViewModel);
    }
}
