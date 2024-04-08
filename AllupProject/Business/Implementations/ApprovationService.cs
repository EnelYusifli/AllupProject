using AllupProject.Business.Interfaces;
using AllupProject.CustomExceptions.Common;
using AllupProject.DAL;
using AllupProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AllupProject.Business.Implementations;

public class ApprovationService:IApprovationService
{
    private readonly AllupDbContext _context;

    public ApprovationService(AllupDbContext context)
    {
        _context = context;
    }

    public async Task Approve(int orderId)
    {
        Order order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
        if (order == null)
        {
            throw new EntityCannotBeFoundException();
        }
        order.IsApproved = true;
        order.IsDeactive = true;
        order.ModifiedDate = DateTime.Now.AddHours(4);
        await _context.SaveChangesAsync();
    }
    public async Task NotApprove(int orderId)
    {
        Order order = await _context.Orders.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.Id == orderId);
        if (order == null)
        {
            throw new Exception();
        }
        order.IsApproved = false;
        order.IsDeactive = true;
        order.ModifiedDate = DateTime.Now.AddHours(4);
        foreach (var oi in order.OrderItems)
        {
            Product product = await _context.Products.Where(x => x.Id == oi.ProductId).FirstOrDefaultAsync();
            if (product == null)
            {
                throw new EntityCannotBeFoundException();
            }
            product.StockCount += oi.Count;
        }
        await _context.SaveChangesAsync();
    }
}
