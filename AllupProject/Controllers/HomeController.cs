using AllupProject.Business.Implementations;
using AllupProject.Business.Interfaces;
using AllupProject.CustomExceptions.Common;
using AllupProject.DAL;
using AllupProject.Models;
using AllupProject.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

namespace AllupProject.Controllers;

public class HomeController : Controller
{
    private readonly AllupDbContext _context;
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;

    public HomeController(AllupDbContext context, ICartService cartService, IOrderService orderService)
    {
        _context = context;
        _cartService = cartService;
        _orderService = orderService;
    }
    public async Task<IActionResult> Index()
    {
        HomeViewModel homeViewModel = new HomeViewModel()
        {
            Sliders = await _context.Sliders.Where(x => x.IsDeactive == false).ToListAsync(),
            Banners = await _context.Banners.Where(x => x.IsDeactive == false).ToListAsync(),
            Categories = await _context.Categories.Where(x => x.IsDeactive == false).ToListAsync(),
            Blogs = await _context.Blogs.Where(x => x.IsDeactive == false).ToListAsync(),
            Products = await _context.Products.Where(x => x.IsDeactive == false).Include(x => x.ProductImages).Include(x => x.Category).ToListAsync()
        };
        return View(homeViewModel);
    }
    public IActionResult About() => View();
    public IActionResult Blog() => View();
    public IActionResult Contact() => View();
    public IActionResult Shop() => View();
    public async Task<IActionResult> Detail(int productId) {
        Product product=await _context.Products.Include(x=>x.ProductImages).Include(x=>x.Category).FirstOrDefaultAsync(x=>x.Id==productId);
        if (product is null) return NotFound();
        return View(product);
    }

    public async Task<IActionResult> AddToCart(int productId)
    {
        try
        {
            await _cartService.AddToCart(HttpContext, productId);
            return RedirectToAction("Cart");

        }
        catch (MoreThanMaxLengthException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return RedirectToAction("Cart");
        } 
        catch (EntityCannotBeFoundException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return RedirectToAction("Cart");

        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return RedirectToAction("Cart");

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
            await _cartService.DeleteItemFromCart(HttpContext, productId);
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
            List<CartItemViewModel> cartItems = await _cartService.Cart(HttpContext);
            ViewBag.Products = _context.Products.Include(x => x.ProductImages).ToList();
            return View(cartItems);
        }
        catch (Exception)
        {
            return NotFound();
        }

    }

    public async Task<IActionResult> Order()
    {
        try
        {
            List<CartItemViewModel> orderItems = await _orderService.GetOrderItems(HttpContext);
            ViewBag.Products = _context.Products.ToList();
            ViewBag.OrderProducts = orderItems;
            return View();
        }
        catch (Exception)
        {
            return NotFound();
        }

    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Order(Order order)
    {
        List<CartItemViewModel> orderItems = await _orderService.GetOrderItems(HttpContext);
        ViewBag.Products = _context.Products.ToList();
        ViewBag.OrderProducts = orderItems;

        if (!ModelState.IsValid)
            return View();

        try
        {
            await _orderService.CreateOrderAsync(HttpContext, orderItems, order);
            return RedirectToAction("Index");
        }
        catch (MoreThanMaxLengthException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View();
        }
    }

}

