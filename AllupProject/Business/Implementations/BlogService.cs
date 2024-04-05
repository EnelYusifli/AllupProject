using AllupProject.Business.Interfaces;
using AllupProject.CustomExceptions.Common;
using AllupProject.DAL;
using AllupProject.Extensions;
using AllupProject.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AllupProject.Business.Implementations;

public class BlogService:IBlogService
{
    private readonly AllupDbContext _context;
    private readonly IWebHostEnvironment _env;

    public BlogService(AllupDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }
    public async Task CreateAsync(Blog blog)
    {
        if (blog.ImageFile is null)
            throw new ImageCannotBeNullException("Image must be uploaded", "ImageFile");

        if (!(blog.ImageFile.ContentType == "image/jpeg" || blog.ImageFile.ContentType == "image/png"))
            throw new InvalidContentTypeException("Content type must be jpeg or png", "ImageFile");

        if (blog.ImageFile.Length > 2097152)
            throw new MoreThanMaxLengthException("Size must be less than 2 mb", "ImageFile");

        blog.CreatedDate = DateTime.UtcNow.AddHours(4);
        blog.ModifiedDate = DateTime.UtcNow.AddHours(4);
        blog.ImageUrl = blog.ImageFile.SaveFile(_env.WebRootPath, "uploads/blogs");
        await _context.Blogs.AddAsync(blog);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        Blog? blg = await _context.Blogs.FirstOrDefaultAsync(x => x.Id == id);
        if (blg == null)
            throw new EntityCannotBeFoundException("Blog cannot be found");
        FileExtension.DeleteFile(_env.WebRootPath, "uploads/blogs", blg.ImageUrl);
        _context.Blogs.Remove(blg);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Blog>> GetAllAsync(Expression<Func<Blog, bool>>? expression = null)
    {
        var query = _context.Blogs.AsQueryable();

        return expression is not null
                ? await query.Where(expression).ToListAsync()
                : await query.ToListAsync();
    }
    public IQueryable<Blog> GetAllAsQueryableAsync(Expression<Func<Blog, bool>>? expression = null)
    {
        var query = _context.Blogs
            .AsQueryable();

        return expression is not null
                ? query.Where(expression).AsQueryable()
                : query.AsQueryable();
    }
    public async Task<Blog> GetByIdAsync(int id)
    {
        var data = await _context.Blogs.FindAsync(id);
        if (data is null) throw new EntityCannotBeFoundException();

        return data;
    }

    public async Task<Blog> GetSingleAsync(Expression<Func<Blog, bool>>? expression = null)
    {
        var query = _context.Blogs.AsQueryable();

        return expression is not null
                ? await query.Where(expression).FirstOrDefaultAsync()
                : await query.FirstOrDefaultAsync();
    }
    public async Task UpdateAsync(Blog blog)
    {
        Blog blg = _context.Blogs.FirstOrDefault(x => x.Id == blog.Id);
        if (blog == null)
            throw new EntityCannotBeFoundException("Blog cannot be found");
        if (blog.ImageFile is not null)
        {
            if (!(blog.ImageFile.ContentType == "image/jpeg" || blog.ImageFile.ContentType == "image/png"))
                throw new InvalidContentTypeException("Content type must be jpeg or png", "ImageFile");

            if (blog.ImageFile.Length > 2097152)
                throw new MoreThanMaxLengthException("Size must be less than 2 mb", "ImageFile");
            FileExtension.DeleteFile(_env.WebRootPath, "uploads/blogs", blg.ImageUrl);
            blg.ImageUrl = blog.ImageFile.SaveFile(_env.WebRootPath, "uploads/blogs");
        }
        blg.ModifiedDate = DateTime.UtcNow.AddHours(4);
        blg.Title = blog.Title;
        blg.Desc = blog.Desc;
        blg.Date = blog.Date;
        blg.IsDeactive = blog.IsDeactive;
        await _context.SaveChangesAsync();
    }
}
