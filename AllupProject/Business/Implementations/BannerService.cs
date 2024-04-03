using AllupProject.Business.Interfaces;
using AllupProject.CustomExceptions.Common;
using AllupProject.DAL;
using AllupProject.Extensions;
using AllupProject.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AllupProject.Business.Implementations;

public class BannerService:IBannerService
{
    private readonly AllupDbContext _context;
    private readonly IWebHostEnvironment _env;

    public BannerService(AllupDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }
    public async Task CreateAsync(Banner banner)
    {
        if (banner.ImageFile is null)
            throw new ImageCannotBeNullException("Image must be uploaded", "ImageFile");

        if (!(banner.ImageFile.ContentType == "image/jpeg" || banner.ImageFile.ContentType == "image/png"))
            throw new InvalidContentTypeException("Content type must be jpeg or png", "ImageFile");

        if (banner.ImageFile.Length > 2097152)
            throw new MoreThanMaxLengthException("Size must be less than 2 mb", "ImageFile");

        banner.CreatedDate = DateTime.UtcNow.AddHours(4);
        banner.ModifiedDate = DateTime.UtcNow.AddHours(4);
        banner.ImageUrl = banner.ImageFile.SaveFile(_env.WebRootPath, "uploads/banners");
        await _context.Banners.AddAsync(banner);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        Banner? sld = await _context.Banners.FirstOrDefaultAsync(x => x.Id == id);
        if (sld == null)
            throw new EntityCannotBeFoundException("Slider cannot be found");
        FileExtension.DeleteFile(_env.WebRootPath, "uploads/banners", sld.ImageUrl);
        _context.Banners.Remove(sld);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Banner>> GetAllAsync(Expression<Func<Banner, bool>>? expression = null)
    {
        var query = _context.Banners.AsQueryable();

        return expression is not null
                ? await query.Where(expression).ToListAsync()
                : await query.ToListAsync();
    }

    public async Task<Banner> GetByIdAsync(int id)
    {
        var data = await _context.Banners.FindAsync(id);
        if (data is null) throw new EntityCannotBeFoundException();

        return data;
    }

    public async Task<Banner> GetSingleAsync(Expression<Func<Banner, bool>>? expression = null)
    {
        var query = _context.Banners.AsQueryable();

        return expression is not null
                ? await query.Where(expression).FirstOrDefaultAsync()
                : await query.FirstOrDefaultAsync();
    }
    public async Task UpdateAsync(Banner banner)
    {
        Banner sld = _context.Banners.FirstOrDefault(x => x.Id == banner.Id);
        if (banner == null)
            throw new EntityCannotBeFoundException("Slider cannot be found");
        if (banner.ImageFile is not null)
        {
            if (!(banner.ImageFile.ContentType == "image/jpeg" || banner.ImageFile.ContentType == "image/png"))
                throw new InvalidContentTypeException("Content type must be jpeg or png", "ImageFile");

            if (banner.ImageFile.Length > 2097152)
                throw new MoreThanMaxLengthException("Size must be less than 2 mb", "ImageFile");
            FileExtension.DeleteFile(_env.WebRootPath, "uploads/banners", sld.ImageUrl);
            sld.ImageUrl = banner.ImageFile.SaveFile(_env.WebRootPath, "uploads/banners");
        }
        sld.ModifiedDate = DateTime.UtcNow.AddHours(4);
        sld.RedirectUrl = banner.RedirectUrl;
        await _context.SaveChangesAsync();
    }
}
