using AllupProject.Business.Interfaces;
using AllupProject.CustomExceptions.Common;
using AllupProject.DAL;
using AllupProject.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AllupProject.Business.Implementations;

public class CategoryService : ICategoryService
{
    private readonly AllupDbContext _context;

    public CategoryService(AllupDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(Category category)
    {
        if (_context.Categories.Any(x => x.Name.ToLower() == category.Name.ToLower()))
            throw new NameAlreadyExistException("Name", "Category name is already exist!");

        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var data = await _context.Categories.FindAsync(id);
        if (data is null) throw new EntityCannotBeFoundException("Category not found!");

        _context.Remove(data);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Category>> GetAllAsync(Expression<Func<Category, bool>>? expression = null, params string[] includes)
    {
        var query = _context.Categories.AsQueryable();

        query = _getIncludes(query, includes);

        return expression is not null
                ? await query.Where(expression).ToListAsync() 
                : await query.ToListAsync(); 
    }

    public async Task<Category> GetByIdAsync(int id)
    {
        var data = await _context.Categories.FindAsync(id);
        if (data is null) throw new EntityCannotBeFoundException();

        return data;
    }

    public async Task<Category> GetSingleAsync(Expression<Func<Category, bool>>? expression = null)
    {
        var query = _context.Categories.AsQueryable();

        return expression is not null
                ? await query.Where(expression).FirstOrDefaultAsync()
                : await query.FirstOrDefaultAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        var data = await _context.Categories.FindAsync(id);
        if (data is null) throw new EntityCannotBeFoundException();
        data.IsDeactive = !data.IsDeactive;

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Category category)
    {
        var existData = await _context.Categories.FindAsync(category.Id);
        if (existData is null) throw new EntityCannotBeFoundException("Category not found!");
        if (_context.Categories.Any(x => x.Name.ToLower() == category.Name.ToLower())
            && existData.Name != category.Name)
            throw new NameAlreadyExistException("Name", "Category name is already exist!");

        existData.Name = category.Name;
        existData.IsDeactive = category.IsDeactive;
        await _context.SaveChangesAsync();
    }


    private IQueryable<Category> _getIncludes(IQueryable<Category> query, params string[] includes)
    {
        if (includes is not null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        return query;
    }
}
