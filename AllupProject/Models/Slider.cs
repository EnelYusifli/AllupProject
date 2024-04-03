using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllupProject.Models;

public class Slider:BaseEntity
{
    [Required]
    [StringLength(20)]
    public string Title1 { get; set; }
    [Required]
    [StringLength(20)]
    public string Title2 { get; set; }
    [Required]
    [StringLength(100)]
    public string Desc { get; set; }
    public string? RedirectUrl { get; set; }
    [Required]
    [StringLength(30)]
    public string RedirectUrlText { get; set; }
    public string? ImageUrl { get; set; }
    [NotMapped]
    public IFormFile? ImageFile { get; set; }
}
