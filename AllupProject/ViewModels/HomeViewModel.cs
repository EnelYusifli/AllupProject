using AllupProject.Models;

namespace AllupProject.ViewModels;

public class HomeViewModel
{
    public List<Slider>? Sliders { get; set; }
    public List<Banner>? Banners { get; set; }
    public List<Product>? Products { get; set; }
    public List<Category>? Categories { get; set; }
}
