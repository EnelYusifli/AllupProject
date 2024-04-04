
using AllupProject.Models;
using System.Linq.Expressions;

namespace AllupProject.Business.Interfaces;

public interface IProductService
{
    Task<Product> GetByIdAsync(int id);
    Task<Product> GetSingleAsync(Expression<Func<Product, bool>>? expression = null);
    Task<List<Product>> GetAllAsync(Expression<Func<Product, bool>>? expression = null);
    Task CreateAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(int id);
    Task SoftDeleteAsync(int id);
    Task HandleDetailImage(string fileName);
}
