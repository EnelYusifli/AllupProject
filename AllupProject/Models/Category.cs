using System.ComponentModel.DataAnnotations;

namespace AllupProject.Models;

public class Category:BaseEntity
{
    [Required]
    [StringLength(20)]
    public string Name { get; set; }
    public List<Product>? Products { get; set; }
}
