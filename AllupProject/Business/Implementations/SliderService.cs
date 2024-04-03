using AllupProject.Business.Interfaces;
using AllupProject.CustomExceptions.Common;
using AllupProject.DAL;
using AllupProject.Extensions;
using AllupProject.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AllupProject.Business.Implementations;

public class SliderService : ISliderService
{
    private readonly AllupDbContext _context;
    private readonly IWebHostEnvironment _env;

    public SliderService(AllupDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }
    public async Task CreateAsync(Slider slider)
    {
        if (slider.ImageFile is null)
            throw new ImageCannotBeNullException("Image must be uploaded", "ImageFile");

        if (!(slider.ImageFile.ContentType == "image/jpeg" || slider.ImageFile.ContentType == "image/png"))
            throw new InvalidContentTypeException("Content type must be jpeg or png", "ImageFile");
           
        if (slider.ImageFile.Length > 2097152) 
            throw new MoreThanMaxLengthException("Size must be less than 2 mb","ImageFile");
           
        slider.CreatedDate = DateTime.UtcNow.AddHours(4);
        slider.ModifiedDate = DateTime.UtcNow.AddHours(4);
        slider.ImageUrl = slider.ImageFile.SaveFile(_env.WebRootPath, "uploads/sliders");
        await _context.Sliders.AddAsync(slider);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        Slider? sld =await _context.Sliders.FirstOrDefaultAsync(x => x.Id == id);
        if (sld == null)
            throw new EntityCannotBeFoundException("Slider cannot be found");
        FileExtension.DeleteFile(_env.WebRootPath, "uploads/sliders", sld.ImageUrl);
        _context.Sliders.Remove(sld);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Slider>> GetAllAsync(Expression<Func<Slider, bool>>? expression = null)
    {
        var query = _context.Sliders.AsQueryable();

        return expression is not null
                ? await query.Where(expression).ToListAsync()
                : await query.ToListAsync();
    }

    public async Task<Slider> GetByIdAsync(int id)
    {
        var data = await _context.Sliders.FindAsync(id);
        if (data is null) throw new EntityCannotBeFoundException();

        return data;
    }

    public async Task<Slider> GetSingleAsync(Expression<Func<Slider, bool>>? expression = null)
    {
        var query = _context.Sliders.AsQueryable();

        return expression is not null
                ? await query.Where(expression).FirstOrDefaultAsync()
                : await query.FirstOrDefaultAsync();
    }
    public async Task UpdateAsync(Slider slider)
    {
        Slider sld = _context.Sliders.FirstOrDefault(x => x.Id == slider.Id);
        if (slider == null) 
            throw new EntityCannotBeFoundException("Slider cannot be found");
        if (slider.ImageFile is not null)
        {
            if (!(slider.ImageFile.ContentType == "image/jpeg" || slider.ImageFile.ContentType == "image/png"))
                throw new InvalidContentTypeException("Content type must be jpeg or png", "ImageFile");

            if (slider.ImageFile.Length > 2097152)
                throw new MoreThanMaxLengthException("Size must be less than 2 mb", "ImageFile");
            FileExtension.DeleteFile(_env.WebRootPath, "uploads/sliders", sld.ImageUrl);
            sld.ImageUrl=slider.ImageFile.SaveFile(_env.WebRootPath, "uploads/sliders");
        }
        sld.ModifiedDate = DateTime.UtcNow.AddHours(4);
        sld.Title1 = slider.Title1;
        sld.Title2 = slider.Title2;
        sld.Desc = slider.Desc;
        sld.IsDeactive = slider.IsDeactive;
        sld.RedirectUrlText = slider.RedirectUrlText;
        sld.RedirectUrl = slider.RedirectUrl;
        await _context.SaveChangesAsync();
    }
}
