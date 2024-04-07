using AllupProject.Business.Interfaces;
using AllupProject.DAL;
using AllupProject.Models;
using AllupProject.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics.Metrics;
using System.Net.Mail;

namespace AllupProject.Business.Implementations;

public class OrderService:IOrderService
{
    private readonly AllupDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public OrderService(AllupDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<List<CartItemViewModel>> GetOrderItems(HttpContext httpContext)
    {
        List<CartItemViewModel> cartItems = new List<CartItemViewModel>();
        List<CartItem> userCartItems = new List<CartItem>();
        IdentityUser user = null;

        if (httpContext.User.Identity.IsAuthenticated)
        {
            user = await _userManager.FindByNameAsync(httpContext.User.Identity.Name);
        }

        if (user is not null)
        {
            userCartItems = await _context.CartItems.Where(bi => bi.AppUserId == user.Id && bi.IsDeactive == false).ToListAsync();
            cartItems = userCartItems.Select(ubi => new CartItemViewModel() { ProductId = ubi.ProductId, Count = ubi.Count }).ToList();
        }
        else
        {
            var cartItemsStr = httpContext.Request.Cookies["CartItems"];

            if (cartItemsStr is not null)
            {
                cartItems = JsonConvert.DeserializeObject<List<CartItemViewModel>>(cartItemsStr);
            }
        };
        return cartItems;
    }
    public async Task CreateOrderAsync(HttpContext httpContext,List<CartItemViewModel> items,Order order)
    {
        IdentityUser appUser = null;

        if (httpContext.User.Identity.IsAuthenticated)
        {
            appUser = await _userManager.FindByNameAsync(httpContext.User.Identity.Name);
        }
        Order newOrder = new()
        {
            FirstName = order.FirstName,
            LastName = order.LastName,
            EmailAddress = order.EmailAddress,
            Address1 = order.Address1,
            Address2 = order.Address2,
            Country = order.Country,
            City = order.City,
            IsApproved = false,
            IsDeactive = false,
            CreatedDate = DateTime.UtcNow.AddHours(4),
            ModifiedDate = DateTime.UtcNow.AddHours(4)
        };
        if (appUser is not null)
        {
            newOrder.AppUserId = appUser.Id;
        }
        await _context.Orders.AddAsync(newOrder);
        foreach (var item in items)
        {
            OrderItem orderItem = new()
            {
                ProductId = item.ProductId,
                Count = item.Count,
                Order = newOrder,
                IsDeactive = false,
                CreatedDate = DateTime.UtcNow.AddHours(4),
                ModifiedDate = DateTime.UtcNow.AddHours(4)
            };
            await _context.OrderItems.AddAsync(orderItem);
        }
        await _context.SaveChangesAsync();
    } 
}
