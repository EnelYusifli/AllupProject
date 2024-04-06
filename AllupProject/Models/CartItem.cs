using Microsoft.AspNetCore.Identity;

namespace AllupProject.Models;

public class CartItem:BaseEntity
{
        public string AppUserId { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
        public Product Product { get; set; }
        public IdentityUser AppUser { get; set; }
    
}
