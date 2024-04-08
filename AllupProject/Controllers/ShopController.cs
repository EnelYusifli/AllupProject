using AllupProject.Business.Interfaces;
using AllupProject.DAL;
using AllupProject.Helpers;
using AllupProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace AllupProject.Controllers;

public class ShopController : Controller
{
    private readonly IProductService _productService;
    private readonly AllupDbContext _context;

    public ShopController(IProductService productService,AllupDbContext context)
    {
        _productService = productService;
        _context = context;
    }
    public IActionResult Index(int? categoryId,int? minPrice,int? maxPrice,bool? isFeat,bool? isNew,bool? isBest,string? searchStr, int page = 1,int itemPerPage=3)
    {
        IQueryable<Product> products = null;
        ViewBag.Categories=_context.Categories.ToList();
        if (categoryId is null)
        {
            products = _productService.GetAllAsQueryableAsync();
        }
        else
        {
        products = _productService.GetAllAsQueryableAsync(x=>x.CategoryId==categoryId);
        }
        if(searchStr is not null)
        {
            products=products.Where(x=>x.Title.Contains(searchStr));
        }
        if(minPrice is not null && maxPrice is not null)
        {
            products = products.Where(x => x.SalePrice >= minPrice&&x.SalePrice<=maxPrice);
        }
        if(isFeat is not null && isFeat==true)
        {
            products = products.Where(x => x.IsFeatured == true);
        }
        if (isNew is not null && isNew == true)
        {
            products = products.Where(x => x.IsNew == true);
        }
        if (isBest is not null && isBest == true)
        {
            products = products.Where(x => x.IsBestSeller == true);
        }
        var paginatedDatas = PaginatedList<Product>.Create(products, itemPerPage, page);
        return View(paginatedDatas);
    }
}
