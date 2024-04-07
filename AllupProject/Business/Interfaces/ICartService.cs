using AllupProject.ViewModels;

namespace AllupProject.Business.Interfaces;

public interface ICartService
{
    public Task AddToCart(HttpContext httpContext,int productId,int quantity=1);
    public Task RemoveItemFromCart(HttpContext httpContext,int productId);
    public Task DeleteItemFromCart(HttpContext httpContext, int productId);
    public Task<List<CartItemViewModel>> Cart(HttpContext httpContext);

}
