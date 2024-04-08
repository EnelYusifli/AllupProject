using AllupProject.Business.Interfaces;
using AllupProject.DAL;
using AllupProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AllupProject.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = "SuperAdmin,Admin")]

public class ApprovationController : Controller
{
    private readonly AllupDbContext _context;
    private readonly IApprovationService _approvationService;

    public ApprovationController(AllupDbContext context,IApprovationService approvationService)
    {
        _context = context;
        _approvationService = approvationService;
    }
    public async Task<IActionResult> Index()
    {
        List<Order> orders = await _context.Orders.Where(x=>x.IsDeactive==false).Include(x=>x.OrderItems).ToListAsync();
        ViewBag.Products =await _context.Products.ToListAsync();
        return View(orders);
    }
    public async Task<IActionResult> Approve(int orderId)
    {
        try
        {
           await _approvationService.Approve(orderId);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View("Index");
        }
        
    }
    public async Task<IActionResult> NotApprove(int orderId)
    {
        try
        {
           await _approvationService.NotApprove(orderId);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View("Index");
        }
    }
}
