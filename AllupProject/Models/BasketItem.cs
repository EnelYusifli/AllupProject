using Microsoft.AspNetCore.Identity;

namespace AllupProject.Models;

public class BasketItem : BaseEntity
{
	public string IdentityUserId { get; set; }
	public int BookId { get; set; }
	public int Count { get; set; }
	public Product Product { get; set; }
	public IdentityUser IdentityUser { get; set; }
}
