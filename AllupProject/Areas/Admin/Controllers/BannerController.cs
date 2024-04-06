using AllupProject.Business.Implementations;
using AllupProject.Business.Interfaces;
using AllupProject.CustomExceptions.Common;
using AllupProject.Helpers;
using AllupProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllupProject.Areas.Admin.Controllers;
[Area("Admin")]
public class BannerController : Controller
{
    private readonly IBannerService _bannerService;

    public BannerController(IBannerService bannerService)
    {
        _bannerService = bannerService;
    }
    public IActionResult Index(int page=1)
    {
        var banners = _bannerService.GetAllAsQueryableAsync();
        var paginatedDatas = PaginatedList<Banner>.Create(banners, 5, page);
        return View(paginatedDatas);
    }
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Banner banner)
    {
        if (!ModelState.IsValid) return View();
        try
        {
            await _bannerService.CreateAsync(banner);
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
            Banner banner = await _bannerService.GetByIdAsync(id);
            return View(banner);
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
    public async Task<IActionResult> Update(Banner banner)
    {
        if (!ModelState.IsValid) return View(banner);
        try
        {
            await _bannerService.UpdateAsync(banner);
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
            await _bannerService.DeleteAsync(id);
            return Ok();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return NotFound();
        }
    }
}
