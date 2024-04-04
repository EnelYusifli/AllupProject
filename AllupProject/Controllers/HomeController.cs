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
            Sliders = await _context.Sliders.Where(x=>x.IsDeactive==false).ToListAsync(),
            Banners = await _context.Banners.Where(x => x.IsDeactive == false).ToListAsync(),
            Categories = await _context.Categories.Where(x => x.IsDeactive == false).ToListAsync(),
            Products = await _context.Products.Where(x => x.IsDeactive == false).Include(x=>x.ProductImages).Include(x=>x.Category).ToListAsync()
        };
        return View(homeViewModel);
    }
    public IActionResult About() => View();

}
