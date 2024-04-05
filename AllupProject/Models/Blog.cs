using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AllupProject.Models;

public class Blog:BaseEntity
{
    [Required]
    [StringLength(20)]
    public string Title { get; set; }
    [Required]
    [StringLength(100)]
    public string Desc { get; set; }
    [Required]
    [StringLength(10)]
    public string Date { get; set; }
    public string? ImageUrl { get; set; }
    [NotMapped]
    public IFormFile? ImageFile { get; set; }
}
