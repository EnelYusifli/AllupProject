using AllupProject.Business.Implementations;
using AllupProject.Business.Interfaces;
using AllupProject.DAL;
using AllupProject.Models;
using AllupProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AllupProject.Controllers;

public class HomeController : Controller
{
    private readonly AllupDbContext _context;
    private readonly ICartService _cartService;

    public HomeController(AllupDbContext context, ICartService cartService)
    {
        _context = context;
        _cartService = cartService;
    }
    public async Task<IActionResult> Index()
    {
        HomeViewModel homeViewModel = new HomeViewModel()
        {
            Sliders = await _context.Sliders.Where(x=>x.IsDeactive==false).ToListAsync(),
            Banners = await _context.Banners.Where(x => x.IsDeactive == false).ToListAsync(),
            Categories = await _context.Categories.Where(x => x.IsDeactive == false).ToListAsync(),
            Blogs = await _context.Blogs.Where(x => x.IsDeactive == false).ToListAsync(),
            Products = await _context.Products.Where(x => x.IsDeactive == false).Include(x=>x.ProductImages).Include(x=>x.Category).ToListAsync()
        };
        return View(homeViewModel);
    }
    public IActionResult About() => View();
    public IActionResult Blog() => View();
    public IActionResult Contact() => View();
    public IActionResult Detail() => PartialView("_ProductDetailPartial");

    public async Task<IActionResult> AddToCart(int productId)
    {
        try
        {
            await _cartService.AddToCart(HttpContext, productId);
            return RedirectToAction("Cart");

        }
        catch (Exception)
        {
            return NotFound();
        }

    }

    public async Task<IActionResult> RemoveItemFromCart(int productId)
    {
        try
        {
            await _cartService.RemoveItemFromCart(HttpContext, productId);
            return RedirectToAction("Cart");

        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    public async Task<IActionResult> DeleteItemFromCart(int productId)
    {
        try
        {
            await _cartService.RemoveItemFromCart(HttpContext, productId);
            return RedirectToAction("Cart");

        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    public async Task<IActionResult> Cart()
    {
        try
        {
           List<CartItemViewModel> cartItems= await _cartService.Cart(HttpContext);
        ViewBag.Products = _context.Products.Include(x => x.ProductImages).ToList();
        return View(cartItems);
        }
        catch (Exception)
        {
            return NotFound();
        }

    }
}
