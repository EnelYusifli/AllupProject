using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AllupProject.Business.Interfaces;
using AllupProject.CustomExceptions.Common;
using AllupProject.DAL;
using AllupProject.Extensions;
using AllupProject.Models;
using System.Net;

namespace AllupProject.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly AllupDbContext _context;

    public ProductController(IProductService productService,AllupDbContext context)
    {
        _productService = productService;
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        List<Product> Products = await _productService.GetAllAsync();
        return View(Products);
    }
    public IActionResult Create()
    {
        ViewBag.Categories = _context.Categories.ToList();
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        ViewBag.Categories = _context.Categories.ToList();
        if (!ModelState.IsValid) return View(product);
        try
        {
            await _productService.CreateAsync(product);
        }
        catch (ImageCannotBeNullException ex)
        {
            ModelState.AddModelError(ex.Property, ex.Message);
            return View(product);
        }
        catch (InvalidContentTypeException ex)
        {
            ModelState.AddModelError(ex.Property, ex.Message);
            return View(product);
        }
        catch (MoreThanMaxLengthException ex)
        {
            ModelState.AddModelError(ex.Property, ex.Message);
            return View(product);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(product);
        }
        return RedirectToAction("Index");
    }
    public async Task<IActionResult> Update(int id)
    {
        ViewBag.Categories = _context.Categories.ToList();
        Product? Product = await _productService.GetByIdAsync(id);
        if (Product == null) throw new Exception();
        return View(Product);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(Product product)
    {
        ViewBag.Categories = await _context.Categories.ToListAsync();
        ViewBag.ProductImages = await _context.ProductImages.ToListAsync();
        if (!ModelState.IsValid) return View(product);
        try
        {
            await _productService.UpdateAsync(product);
        }
        catch (ImageCannotBeNullException ex)
        {
            ModelState.AddModelError(ex.Property, ex.Message);
            return View(product);
        }
        catch (InvalidContentTypeException ex)
        {
            ModelState.AddModelError(ex.Property, ex.Message);
            return View(product);
        }
        catch (MoreThanMaxLengthException ex)
        {
            ModelState.AddModelError(ex.Property, ex.Message);
            return View(product);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(product);
        }
        return RedirectToAction("Index");
    }
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _productService.DeleteAsync(id);
            return Ok();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return NotFound();
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Update(string filename)
    {
        try
        {
            await _productService.HandleDetailImage(filename);
            return Ok();
        }
        catch(EntityCannotBeFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

}
