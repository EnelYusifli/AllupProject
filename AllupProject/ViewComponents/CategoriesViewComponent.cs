using AllupProject.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AllupProject.ViewComponents;

public class CategoriesViewComponent:ViewComponent
{
    private readonly ICategoryService _categoryService;

    public CategoriesViewComponent(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var categories = await _categoryService.GetAllAsync();
        return View(categories);
    }
}
