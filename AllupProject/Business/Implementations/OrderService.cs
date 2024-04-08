using AllupProject.Business.Interfaces;
using AllupProject.CustomExceptions.Common;
using AllupProject.DAL;
using AllupProject.Models;
using AllupProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AllupProject.Business.Implementations;

public class OrderService:IOrderService
{
    private readonly AllupDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ICartService _cartService;

    public OrderService(AllupDbContext context, UserManager<IdentityUser> userManager,ICartService cartService)
    {
        _context = context;
        _userManager = userManager;
        _cartService = cartService;
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
        foreach (var item in items)
        {
            Product? product = await _context.Products.FirstOrDefaultAsync(x => x.Id == item.ProductId);
            if (product == null)
            {
                throw new EntityCannotBeFoundException();
            }
            if (product.IsDeactive == true) throw new EntityCannotBeFoundException();
            if (product.StockCount < item.Count)
            {
                throw new MoreThanMaxLengthException();
            }
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
            product.StockCount -= item.Count;
        await _context.Orders.AddAsync(newOrder);
            await _cartService.DeleteItemFromCart(httpContext, product.Id);

        }
        await _context.SaveChangesAsync();
    } 
}
