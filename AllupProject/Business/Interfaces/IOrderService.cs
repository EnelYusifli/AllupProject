using AllupProject.Models;
using AllupProject.ViewModels;

namespace AllupProject.Business.Interfaces;

public interface IOrderService
{
    Task<List<CartItemViewModel>> GetOrderItems(HttpContext httpContext);
    Task CreateOrderAsync(HttpContext httpContext,List<CartItemViewModel> items,Order order);
}
