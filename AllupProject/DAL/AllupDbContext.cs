using AllupProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AllupProject.DAL;

public class AllupDbContext : IdentityDbContext
{
    public AllupDbContext(DbContextOptions<AllupDbContext> options) : base(options) { }
    public DbSet<Slider> Sliders { get; set; }  
    public DbSet<Banner> Banners { get; set; }  
}
