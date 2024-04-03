using AllupProject.Models;
using System.Linq.Expressions;

namespace AllupProject.Business.Interfaces;

public interface IBannerService
{
    Task<Banner> GetByIdAsync(int id);
    Task<Banner> GetSingleAsync(Expression<Func<Banner, bool>>? expression = null);
    Task<List<Banner>> GetAllAsync(Expression<Func<Banner, bool>>? expression = null);
    Task CreateAsync(Banner banner);
    Task UpdateAsync(Banner banner);
    Task DeleteAsync(int id);
}
