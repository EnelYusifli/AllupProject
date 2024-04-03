namespace AllupProject.Models;

public class ProductImage:BaseEntity
{
    public int ProductId { get; set; }
    public string Url { get; set; }
    public bool? IsPoster { get; set; }
    public Product? Product { get; set; }
}
