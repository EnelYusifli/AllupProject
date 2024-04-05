using Microsoft.EntityFrameworkCore;
using AllupProject.Business.Interfaces;
using AllupProject.CustomExceptions.Common;
using AllupProject.DAL;
using AllupProject.Extensions;
using AllupProject.Models;
using System.Linq.Expressions;
using System.Net;

namespace AllupProject.Business.Implementations;

public class ProductService : IProductService
{
    private readonly AllupDbContext _context;
    private readonly IWebHostEnvironment _env;

    public ProductService(AllupDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }
    public  async Task CreateAsync(Product product)
    {
        if (product.PosterImgFile is null)
            throw new ImageCannotBeNullException("Image must be uploaded", "PosterImgFile");
        if (product.HoverImgFile is null)
            throw new ImageCannotBeNullException("Image must be uploaded", "HoverImgFile");

        if (!(product.HoverImgFile.ContentType == "image/jpeg" || product.HoverImgFile.ContentType == "image/png"))
            throw new InvalidContentTypeException("Content type must be jpeg or png", "HoverImgFile");
        if (product.HoverImgFile.Length > 2097152)
            throw new MoreThanMaxLengthException("Size type must be less than 2mb", "HoverImgFile");
        ProductImage hoverProductImage = new()
        {
            Product = product,
            Url = product.HoverImgFile.SaveFile(_env.WebRootPath, "uploads/products"),
            IsPoster = false
        };
        await _context.ProductImages.AddAsync(hoverProductImage);

        if (!(product.PosterImgFile.ContentType == "image/jpeg" || product.HoverImgFile.ContentType == "image/png"))
            throw new InvalidContentTypeException("Content type must be jpeg or png", "PosterImgFile");
        if (product.PosterImgFile.Length > 2097152)
            throw new MoreThanMaxLengthException("Size type must be less than 2mb", "PosterImgFile");
        ProductImage posterProductImage = new()
        {
            Product = product,
            Url = product.PosterImgFile.SaveFile(_env.WebRootPath, "uploads/products"),
            IsPoster = true
        };
        await _context.ProductImages.AddAsync(posterProductImage);

        if (product.DetailImgFiles is not null)
        {
            foreach (var img in product.DetailImgFiles)
            {
                if (!(img.ContentType == "image/jpeg" || img.ContentType == "image/png"))
                    throw new InvalidContentTypeException("Content type must be jpeg or png", "DetailImgFiles");
                if (img.Length > 2097152)
                    throw new MoreThanMaxLengthException("Size type must be less than 2mb", "DetailImgFiles");
                ProductImage ProductImage = new()
                {
                    Product = product,
                    Url = img.SaveFile(_env.WebRootPath, "uploads/products"),
                    IsPoster = null
                };
                await _context.ProductImages.AddAsync(ProductImage);
            }
        }
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var existProduct = await GetByIdAsync(id);
        if (existProduct == null) throw new EntityCannotBeFoundException("Product cannot be found");
        foreach (var ProductImg in existProduct.ProductImages)
        {
            FileExtension.DeleteFile(_env.WebRootPath, "uploads/products", ProductImg.Url);
        }
        _context.Remove(existProduct);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Product>> GetAllAsync(Expression<Func<Product, bool>>? expression = null)
    {
        var query = _context.Products
            .Include(x=>x.Category)
            .Include(x=>x.ProductImages)
            .AsQueryable();

        return expression is not null
                ? await query.Where(expression).ToListAsync()
                : await query.ToListAsync();
    }
    public  IQueryable<Product> GetAllAsQueryableAsync(Expression<Func<Product, bool>>? expression = null)
    {
        var query = _context.Products
            .Include(x => x.Category)
            .Include(x => x.ProductImages)
            .AsQueryable();

        return expression is not null
                ?  query.Where(expression).AsQueryable()
                :  query.AsQueryable();
    }
    public async Task<Product> GetByIdAsync(int id)
    {
        var data = await _context.Products
            .Include(x => x.Category)
            .Include(x => x.ProductImages)
            .FirstOrDefaultAsync(x=>x.Id==id);
        if (data is null) throw new EntityCannotBeFoundException();

        return data;
    }

    public async Task<Product> GetSingleAsync(Expression<Func<Product, bool>>? expression = null)
    {
        var query = _context.Products
            .Include(x => x.Category)
            .Include(x => x.ProductImages)
            .AsQueryable();

        return expression is not null
                ? await query.Where(expression).FirstOrDefaultAsync()
                : await query.FirstOrDefaultAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        var data = await _context.Products.FindAsync(id);
        if (data is null) throw new EntityCannotBeFoundException("Product Cannot be Found");
        data.IsDeactive = !data.IsDeactive;

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        if (product is null)
            throw new ArgumentNullException(nameof(Product));

        Product? existProduct = await _context.Products
            .Include(x => x.ProductImages)
            .FirstOrDefaultAsync(x => x.Id == product.Id);

        if (existProduct is null)
            throw new EntityCannotBeFoundException("Product not found.");

        existProduct.Title = product.Title;
        existProduct.Desc = product.Desc;
        existProduct.ProductCode = product.ProductCode;
        existProduct.CostPrice = product.CostPrice;
        existProduct.SalePrice = product.SalePrice;
        existProduct.DiscountPercent = product.DiscountPercent;
        existProduct.StockCount = product.StockCount;
        existProduct.IsFeatured = product.IsFeatured;
        existProduct.IsNew = product.IsNew;
        existProduct.IsBestSeller = product.IsBestSeller;
        existProduct.IsInStock = product.IsInStock;
        existProduct.IsDeactive = product.IsDeactive;
        existProduct.CategoryId = product.CategoryId;

        if (product.HoverImgFile is not null)
            await HandleProductImage(product.HoverImgFile, existProduct.Id, false);

        if (product.PosterImgFile is not null)
            await HandleProductImage(product.PosterImgFile, existProduct.Id, true);

        if (product.DetailImgFiles is not null)
        {
            foreach (var imgFile in product.DetailImgFiles)
            {
                if (!(imgFile.ContentType == "image/jpeg" || imgFile.ContentType == "image/png"))
                    throw new InvalidContentTypeException("Content type must be jpeg or png.", nameof(imgFile));

                if (imgFile.Length > 2097152)
                    throw new MoreThanMaxLengthException("Size must be less than 2mb.", nameof(imgFile));
                ProductImage ProductImage = new ProductImage
                {
                    ProductId = existProduct.Id,
                    Url = imgFile.SaveFile(_env.WebRootPath, "uploads/products"),
                    IsPoster = null
                };
                await _context.ProductImages.AddAsync(ProductImage);
            }
        }
        await _context.SaveChangesAsync();
    }

    private async Task HandleProductImage(IFormFile imgFile, int productId, bool? isPoster)
    {
        if (!(imgFile.ContentType == "image/jpeg" || imgFile.ContentType == "image/png"))
            throw new InvalidContentTypeException("Content type must be jpeg or png.", nameof(imgFile));

        if (imgFile.Length > 2097152)
            throw new MoreThanMaxLengthException("Size must be less than 2mb.", nameof(imgFile));

        var existingImage = await _context.ProductImages.FirstOrDefaultAsync(x => x.ProductId == productId && x.IsPoster == isPoster);
        if (existingImage is not null)
        {
            FileExtension.DeleteFile(_env.WebRootPath, "uploads/products", existingImage.Url);
            _context.ProductImages.Remove(existingImage);
        }

        ProductImage ProductImage = new ProductImage
        {
            ProductId = productId,
            Url = imgFile.SaveFile(_env.WebRootPath, "uploads/products"),
            IsPoster = isPoster
        };
        await _context.ProductImages.AddAsync(ProductImage);
    }

    public async Task HandleDetailImage(string fileName)
    {
        ProductImage ProductImage = await _context.ProductImages.FirstOrDefaultAsync(x => x.Url == fileName);
        if (ProductImage is null) throw new EntityCannotBeFoundException("Image cannot be found");
        FileExtension.DeleteFile(_env.WebRootPath, "uploads/products", ProductImage.Url);
        _context.ProductImages.Remove(ProductImage);
        await _context.SaveChangesAsync();
    }

}
