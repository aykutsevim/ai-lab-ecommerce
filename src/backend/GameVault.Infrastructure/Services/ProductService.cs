using GameVault.Core.DTOs.Common;
using GameVault.Core.DTOs.Products;
using GameVault.Core.Interfaces;
using GameVault.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;

    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ProductListDto>> GetProductsAsync(ProductFilterRequest filter)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Tags)
            .Where(p => p.IsActive)
            .AsQueryable();

        // Apply filters
        if (filter.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == filter.CategoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Brand))
        {
            query = query.Where(p => p.Brand == filter.Brand);
        }

        if (!string.IsNullOrWhiteSpace(filter.Subcategory))
        {
            query = query.Where(p => p.Subcategory == filter.Subcategory);
        }

        if (filter.MinPrice.HasValue)
        {
            query = query.Where(p => p.Price >= filter.MinPrice.Value);
        }

        if (filter.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= filter.MaxPrice.Value);
        }

        if (filter.MinRating.HasValue)
        {
            query = query.Where(p => p.Rating >= filter.MinRating.Value);
        }

        if (filter.InStock.HasValue)
        {
            query = query.Where(p => p.InStock == filter.InStock.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var searchLower = filter.Search.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(searchLower) ||
                p.Brand.ToLower().Contains(searchLower) ||
                p.Description.ToLower().Contains(searchLower));
        }

        if (filter.Tags != null && filter.Tags.Any())
        {
            query = query.Where(p => p.Tags.Any(t => filter.Tags.Contains(t.Tag)));
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply sorting
        query = filter.SortBy.ToLower() switch
        {
            "name" => filter.SortOrder.ToLower() == "asc"
                ? query.OrderBy(p => p.Name)
                : query.OrderByDescending(p => p.Name),
            "price" => filter.SortOrder.ToLower() == "asc"
                ? query.OrderBy(p => p.Price)
                : query.OrderByDescending(p => p.Price),
            "rating" => filter.SortOrder.ToLower() == "asc"
                ? query.OrderBy(p => p.Rating)
                : query.OrderByDescending(p => p.Rating),
            "brand" => filter.SortOrder.ToLower() == "asc"
                ? query.OrderBy(p => p.Brand)
                : query.OrderByDescending(p => p.Brand),
            _ => filter.SortOrder.ToLower() == "asc"
                ? query.OrderBy(p => p.CreatedAt)
                : query.OrderByDescending(p => p.CreatedAt)
        };

        // Apply pagination
        var items = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(p => new ProductListDto
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                Brand = p.Brand,
                ShortDescription = p.ShortDescription,
                Price = p.Price,
                CompareAtPrice = p.CompareAtPrice,
                Currency = p.Currency,
                InStock = p.InStock,
                Rating = p.Rating,
                ReviewCount = p.ReviewCount,
                Subcategory = p.Subcategory,
                ThumbnailUrl = p.Images.OrderBy(i => i.SortOrder).FirstOrDefault() != null
                    ? p.Images.OrderBy(i => i.SortOrder).First().Url
                    : null,
                Tags = p.Tags.Select(t => t.Tag).ToList()
            })
            .ToListAsync();

        return new PagedResult<ProductListDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<ProductDto?> GetProductByIdAsync(Guid id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Variants)
            .Include(p => p.Images)
            .Include(p => p.Specs)
            .Include(p => p.Tags)
            .Where(p => p.Id == id)
            .Select(p => MapToProductDto(p))
            .FirstOrDefaultAsync();
    }

    public async Task<ProductDto?> GetProductBySlugAsync(string slug)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Variants)
            .Include(p => p.Images)
            .Include(p => p.Specs)
            .Include(p => p.Tags)
            .Where(p => p.Slug == slug)
            .Select(p => MapToProductDto(p))
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<string>> GetBrandsAsync()
    {
        return await _context.Products
            .Where(p => p.IsActive)
            .Select(p => p.Brand)
            .Distinct()
            .OrderBy(b => b)
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetTagsAsync()
    {
        return await _context.ProductTags
            .Select(t => t.Tag)
            .Distinct()
            .OrderBy(t => t)
            .ToListAsync();
    }

    private static ProductDto MapToProductDto(Core.Entities.Product p)
    {
        return new ProductDto
        {
            Id = p.Id,
            CategoryId = p.CategoryId,
            CategoryName = p.Category.Name,
            Name = p.Name,
            Slug = p.Slug,
            Brand = p.Brand,
            ShortDescription = p.ShortDescription,
            Description = p.Description,
            Price = p.Price,
            CompareAtPrice = p.CompareAtPrice,
            Currency = p.Currency,
            InStock = p.InStock,
            StockCount = p.StockCount,
            Rating = p.Rating,
            ReviewCount = p.ReviewCount,
            Subcategory = p.Subcategory,
            IsActive = p.IsActive,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt,
            Variants = p.Variants.OrderBy(v => v.SortOrder).Select(v => new ProductVariantDto
            {
                Id = v.Id,
                Name = v.Name,
                Color = v.Color,
                ImageUrl = v.ImageUrl,
                AdditionalPrice = v.AdditionalPrice,
                SortOrder = v.SortOrder
            }).ToList(),
            Images = p.Images.OrderBy(i => i.SortOrder).Select(i => new ProductImageDto
            {
                Id = i.Id,
                Url = i.Url,
                Alt = i.Alt,
                VariantName = i.VariantName,
                SortOrder = i.SortOrder
            }).ToList(),
            Specs = p.Specs.OrderBy(s => s.SortOrder).Select(s => new ProductSpecDto
            {
                Id = s.Id,
                Key = s.Key,
                Value = s.Value,
                SortOrder = s.SortOrder
            }).ToList(),
            Tags = p.Tags.Select(t => t.Tag).ToList()
        };
    }
}
