using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AllupProject.Models;

public class Order:BaseEntity
{
    public IdentityUser? AppUser { get; set; }
    public string? AppUserId { get; set; }
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; }
    [Required]
    [StringLength(50)]
    public string LastName { get; set; }
    [Required]
    [StringLength(50)]
    public string EmailAddress { get; set; }
    [Required]
    [StringLength(150)]
    public string Address1 { get; set; }
    [Required]
    [StringLength(150)]
    public string Address2 { get; set; }
    [Required]
    [StringLength(50)]
    public string Country { get; set; }
    [Required]
    [StringLength(50)]
    public string City { get; set; }
    public bool IsApproved { get; set; } = false;
    public List<OrderItem>? OrderItems { get; set; }
}
