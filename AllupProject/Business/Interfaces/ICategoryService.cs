
using AllupProject.Models;
using System.Linq.Expressions;

namespace AllupProject.Business.Interfaces;

public interface ICategoryService
{
    Task<Category> GetByIdAsync(int id);
    Task<Category> GetSingleAsync(Expression<Func<Category, bool>>? expression = null);
    Task<List<Category>> GetAllAsync(Expression<Func<Category, bool>>? expression = null, params string[] includes);
    IQueryable<Category> GetAllAsQueryableAsync(Expression<Func<Category, bool>>? expression = null);

    Task CreateAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(int id);
    Task SoftDeleteAsync(int id);
}
