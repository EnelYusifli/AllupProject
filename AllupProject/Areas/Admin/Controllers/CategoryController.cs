using AllupProject.Business.Implementations;
using AllupProject.Business.Interfaces;
using AllupProject.CustomExceptions.Common;
using AllupProject.Helpers;
using AllupProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllupProject.Areas.Admin.Controllers;

[Area("admin")]
[Authorize(Roles = "SuperAdmin,Admin")]

public class CategoryController : Controller
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public IActionResult Index(int page=1, int itemPerPage = 5)
    {
        var categories = _categoryService.GetAllAsQueryableAsync();
        var paginatedDatas = PaginatedList<Category>.Create(categories, itemPerPage, page);
        return View(paginatedDatas);
    }
    public IActionResult Create()
        => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category)
    {
        if (!ModelState.IsValid) return View();

        try
        {
            await _categoryService.CreateAsync(category);
        }
        catch (NameAlreadyExistException ex)
        {
            ModelState.AddModelError(ex.PropertyName, ex.Message);
            return View();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View();
        }

        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Update(int id)
    {
        Category category = null;
        try
        {
            category = await _categoryService.GetByIdAsync(id);
        }
        catch (EntityCannotBeFoundException ex)
        {
            return View("Error");
        }
        catch (Exception)
        {

            throw;
        }

        return View(category);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(Category category)
    {
        if (!ModelState.IsValid) return View();

        try
        {
            await _categoryService.UpdateAsync(category);
        }
        catch (NameAlreadyExistException ex)
        {
            ModelState.AddModelError(ex.PropertyName, ex.Message);
            return View();
        }
        catch (EntityCannotBeFoundException ex)
        {
            return View("Error");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View();
        }

        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _categoryService.DeleteAsync(id);
        }
        catch (EntityCannotBeFoundException)
        {
            return NotFound();
        }
        catch (Exception)
        {
            return NotFound();
        }
        return Ok();
    }

}
