using AllupProject.Business.Interfaces;
using AllupProject.DAL;
using AllupProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AllupProject.Areas.Admin.Controllers;
[Area("Admin")]
public class ApprovationController : Controller
{
    private readonly AllupDbContext _context;

    public ApprovationController(AllupDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        List<Order> orders = _context.Orders.Where(x=>x.IsDeactive==false).Include(x=>x.OrderItems).ToList();
        ViewBag.Products = _context.Products.ToList();
        return View(orders);
    }
    public async Task<IActionResult> Approve(int orderId)
    {
        Order order= await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
        if(order == null) {
            throw new Exception();
        }
        order.IsApproved = true;
        order.IsDeactive = true;
        order.ModifiedDate = DateTime.Now.AddHours(4);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index"); 
    }
    public async Task<IActionResult> NotApprove(int orderId)
    {
        Order order = await _context.Orders.Include(x=>x.OrderItems).FirstOrDefaultAsync(x => x.Id == orderId);
        if (order == null)
        {
            throw new Exception();
        }
        order.IsApproved = false;
        order.IsDeactive = true;
        order.ModifiedDate=DateTime.Now.AddHours(4);
        foreach (var oi in order.OrderItems)
        {
            Product product=await _context.Products.Where(x => x.Id == oi.ProductId).FirstOrDefaultAsync();
            if(product == null) {
                throw new Exception();
            }
            product.StockCount += oi.Count;
        }
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}
