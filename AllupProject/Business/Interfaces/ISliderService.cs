using AllupProject.Models;
using System.Linq.Expressions;

namespace AllupProject.Business.Interfaces;

public interface ISliderService
{
    Task<Slider> GetByIdAsync(int id);
    Task<Slider> GetSingleAsync(Expression<Func<Slider, bool>>? expression = null);
    Task<List<Slider>> GetAllAsync(Expression<Func<Slider, bool>>? expression = null);
    Task CreateAsync(Slider slider);
    Task UpdateAsync(Slider slider);
    Task DeleteAsync(int id);

}
