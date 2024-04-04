using Humanizer.Localisation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AllupProject.Models;

public class Product:BaseEntity
{
    public int CategoryId { get; set; }
    [StringLength(100)]
    public string Title { get; set; }
    [StringLength(200)]
    public string Desc { get; set; }
    public string ProductCode { get; set; }
    public double CostPrice { get; set; }
    public double SalePrice { get; set; }
    public double DiscountPercent { get; set; }
    public int StockCount { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsNew { get; set; }
    public bool IsBestSeller { get; set; }
    public bool IsInStock { get; set; }
    public Category? Category { get; set; }
    public List<ProductImage>? ProductImages { get; set; }
    [NotMapped]
    public IFormFile? PosterImgFile { get; set; }
    [NotMapped]
    public IFormFile? HoverImgFile { get; set; }
    [NotMapped]
    public List<IFormFile>? DetailImgFiles { get; set; }
}
