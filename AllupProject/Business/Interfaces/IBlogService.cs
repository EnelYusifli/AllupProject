using AllupProject.Models;
using System.Linq.Expressions;

namespace AllupProject.Business.Interfaces;

public interface IBlogService
{
    Task<Blog> GetByIdAsync(int id);
    Task<Blog> GetSingleAsync(Expression<Func<Blog, bool>>? expression = null);
    Task<List<Blog>> GetAllAsync(Expression<Func<Blog, bool>>? expression = null);
    IQueryable<Blog> GetAllAsQueryableAsync(Expression<Func<Blog, bool>>? expression = null);

    Task CreateAsync(Blog blog);
    Task UpdateAsync(Blog blog);
    Task DeleteAsync(int id);
}
