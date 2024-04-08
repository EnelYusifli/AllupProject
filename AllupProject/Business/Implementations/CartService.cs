using AllupProject.Business.Interfaces;
using AllupProject.CustomExceptions.Common;
using AllupProject.DAL;
using AllupProject.Models;
using AllupProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel;
namespace AllupProject.Business.Implementations;

public class CartService:ICartService
{
    private readonly AllupDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public CartService(AllupDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public async Task AddToCart(HttpContext httpContext,int productId,int count=1)
    {
        if (!await _context.Products.AnyAsync(x => x.Id == productId && x.IsDeactive==false)) throw new EntityCannotBeFoundException();
        Product? product=await _context.Products.FirstOrDefaultAsync(x => x.Id == productId);
        if (product.IsDeactive == true) throw new EntityCannotBeFoundException("Product cannot be found");
       
        List<CartItemViewModel> cartItems = new List<CartItemViewModel>();
        CartItemViewModel cartItem = null;
        CartItem userCartItem = null;
        var cartItemsStr = httpContext.Request.Cookies["CartItems"];

        IdentityUser appUser = null;

        if (httpContext.User.Identity.IsAuthenticated)
        {
            appUser = await _userManager.FindByNameAsync(httpContext.User.Identity.Name);
        }

        if (appUser is null)
        {
            if (cartItemsStr is not null)
            {
                cartItems = JsonConvert.DeserializeObject<List<CartItemViewModel>>(cartItemsStr);

                cartItem = cartItems.FirstOrDefault(x => x.ProductId == productId);

                if (cartItem is not null)
                {
                    if (product.StockCount < cartItem.Count+count)
                    {
                        throw new MoreThanMaxLengthException("More Than Stock Count");
                    }
                    cartItem.Count+=count;
                }
                else
                {
                    cartItem = new CartItemViewModel();

                    if (product.StockCount < cartItem.Count + count)
                    {
                        throw new MoreThanMaxLengthException("More Than Stock Count");
                    }
                    cartItem.ProductId = productId;
                    cartItem.Count = count;

                    cartItems.Add(cartItem);
                }
            }
            else
            {

                cartItem = new CartItemViewModel();

                if (product.StockCount < cartItem.Count + count)
                {
                    throw new MoreThanMaxLengthException("More Than Stock Count");
                }
                cartItem.ProductId = productId;
                cartItem.Count = count;


                cartItems.Add(cartItem);
            }
        }
        else
        {
            userCartItem = await _context.CartItems.FirstOrDefaultAsync(bi => bi.AppUserId == appUser.Id && bi.ProductId == productId);

            if (userCartItem is not null && !userCartItem.IsDeactive)
            {

                if (product.StockCount < userCartItem.Count + count)
                {
                    throw new MoreThanMaxLengthException("More Than Stock Count");
                }
                userCartItem.Count++;
            }
            else
            {
                userCartItem = new CartItem();
                if (product.StockCount < userCartItem.Count + count)
                {
                    throw new MoreThanMaxLengthException("More Than Stock Count");
                }
                userCartItem.ProductId = productId;
                userCartItem.Count = count;
                userCartItem.AppUserId = appUser.Id;
                userCartItem.IsDeactive = false;
                userCartItem. CreatedDate = DateTime.UtcNow;
                userCartItem.ModifiedDate = DateTime.UtcNow;

                await _context.CartItems.AddAsync(userCartItem);
            }
            await _context.SaveChangesAsync();
        }

        cartItemsStr = JsonConvert.SerializeObject(cartItems);

        httpContext.Response.Cookies.Append("CartItems", cartItemsStr);
    }

    public async Task RemoveItemFromCart(HttpContext httpContext, int productId)
    {
        if (!await _context.Products.AnyAsync(x => x.Id == productId)) throw new EntityCannotBeFoundException();

        List<CartItemViewModel> CartItems = new List<CartItemViewModel>();
        CartItemViewModel CartItem = null;
        CartItem userCartItem = null;
        List<CartItem> userCartItems = new List<CartItem>();
        IdentityUser user = null;

        if (httpContext.User.Identity.IsAuthenticated)
        {
            user = await _userManager.FindByNameAsync(httpContext.User.Identity.Name);
        }
        if (user is null)
        {
            var CartItemsStr = httpContext.Request.Cookies["CartItems"];
            if (CartItemsStr is not null)
            {
                CartItems = JsonConvert.DeserializeObject<List<CartItemViewModel>>(CartItemsStr);

                CartItem = CartItems.FirstOrDefault(x => x.ProductId == productId);

                if (CartItem is not null)
                {
                    if (CartItem.Count > 1) CartItem.Count--;
                    else CartItems.Remove(CartItem);

                }
                else throw new EntityCannotBeFoundException();

            }
            else throw new EntityCannotBeFoundException();
            CartItemsStr = JsonConvert.SerializeObject(CartItems);

            httpContext.Response.Cookies.Append("CartItems", CartItemsStr);

        }
        else
        {
            userCartItems = await _context.CartItems.Where(bi => bi.AppUserId == user.Id && bi.IsDeactive == false).ToListAsync();
            userCartItem = userCartItems.FirstOrDefault(x => x.ProductId == productId);

            if (userCartItem is not null)
            {
                if (userCartItem.Count > 1) userCartItem.Count--;
                else _context.CartItems.Remove(userCartItem);
                await _context.SaveChangesAsync();
            }
            else throw new EntityCannotBeFoundException();
        }
    }

    public async Task DeleteItemFromCart(HttpContext httpContext, int productId)
    {
        if (!await _context.Products.AnyAsync(x => x.Id == productId)) throw new EntityCannotBeFoundException();

        List<CartItemViewModel> CartItems = new List<CartItemViewModel>();
        CartItemViewModel CartItem = null;
        CartItem userCartItem = null;
        List<CartItem> userCartItems = new List<CartItem>();
        IdentityUser user = null;

        if (httpContext.User.Identity.IsAuthenticated)
        {
            user = await _userManager.FindByNameAsync(httpContext.User.Identity.Name);
        }
        if (user is null)
        {
            var CartItemsStr = httpContext.Request.Cookies["CartItems"];
            if (CartItemsStr is not null)
            {
                CartItems = JsonConvert.DeserializeObject<List<CartItemViewModel>>(CartItemsStr);

                CartItem = CartItems.FirstOrDefault(x => x.ProductId == productId);

                if (CartItem is not null)
                {
                    CartItems.Remove(CartItem);
                }
                else throw new EntityCannotBeFoundException();

            }
            else throw new EntityCannotBeFoundException();
            CartItemsStr = JsonConvert.SerializeObject(CartItems);

            httpContext.Response.Cookies.Append("CartItems", CartItemsStr);

        }
        else
        {
            userCartItems = await _context.CartItems.Where(bi => bi.AppUserId == user.Id && bi.IsDeactive == false).ToListAsync();
            userCartItem = userCartItems.FirstOrDefault(x => x.ProductId == productId);

            if (userCartItem is not null)
            {
                _context.CartItems.Remove(userCartItem);
                await _context.SaveChangesAsync();
            }
            else throw new EntityCannotBeFoundException();
        }
    }

    public async Task<List<CartItemViewModel>> Cart(HttpContext httpContext)
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
        }
        return cartItems;
    }
}

