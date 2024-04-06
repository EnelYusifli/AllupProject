using AllupProject.Business.Implementations;
using AllupProject.Business.Interfaces;
using AllupProject.CustomExceptions.Common;
using AllupProject.Helpers;
using AllupProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace AllupProject.Areas.Admin.Controllers;
[Area("Admin")]
public class BlogController : Controller
{
    private readonly IBlogService _blogService;

    public BlogController(IBlogService blogService)
    {
        _blogService = blogService;
    }
    public IActionResult Index(int page=1)
    {
        var blogs = _blogService.GetAllAsQueryableAsync();
        var paginatedDatas = PaginatedList<Blog>.Create(blogs, 5, page);
        return View(paginatedDatas);
    }
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Blog blog)
    {
        if (!ModelState.IsValid) return View();
        try
        {
            await _blogService.CreateAsync(blog);
        }
        catch (ImageCannotBeNullException ex)
        {
            ModelState.AddModelError(ex.Property, ex.Message);
            return View();
        }
        catch (InvalidContentTypeException ex)
        {
            ModelState.AddModelError(ex.Property, ex.Message);
            return View();
        }
        catch (MoreThanMaxLengthException ex)
        {
            ModelState.AddModelError(ex.Property, ex.Message);
            return View();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View();
        }
        return RedirectToAction("Index");
    }
    public async Task<IActionResult> Update(int id)
    {
        try
        {
            Blog Blog = await _blogService.GetByIdAsync(id);
            return View(Blog);
        }
        catch (EntityCannotBeFoundException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(Blog blog)
    {
        if (!ModelState.IsValid) return View(blog);
        try
        {
            await _blogService.UpdateAsync(blog);
        }
        catch (InvalidContentTypeException ex)
        {
            ModelState.AddModelError(ex.Property, ex.Message);
            return View();
        }
        catch (MoreThanMaxLengthException ex)
        {
            ModelState.AddModelError(ex.Property, ex.Message);
            return View();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View();
        }
        return RedirectToAction("Index");
    }
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _blogService.DeleteAsync(id);
            return Ok();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return NotFound();
        }
    }
}
