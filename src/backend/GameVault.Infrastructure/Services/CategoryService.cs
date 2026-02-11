using GameVault.Core.DTOs.Categories;
using GameVault.Core.Entities;
using GameVault.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();
        var categoriesList = categories.ToList();

        var productCounts = new Dictionary<Guid, int>();
        foreach (var category in categoriesList)
        {
            var count = await _unitOfWork.Products.CountAsync(p => p.CategoryId == category.Id && p.IsActive);
            productCounts[category.Id] = count;
        }

        return categoriesList
            .OrderBy(c => c.SortOrder)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                SortOrder = c.SortOrder,
                IsActive = c.IsActive,
                ProductCount = productCounts.GetValueOrDefault(c.Id, 0)
            });
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null) return null;

        var productCount = await _unitOfWork.Products.CountAsync(p => p.CategoryId == id && p.IsActive);

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            Description = category.Description,
            ImageUrl = category.ImageUrl,
            SortOrder = category.SortOrder,
            IsActive = category.IsActive,
            ProductCount = productCount
        };
    }

    public async Task<CategoryDto?> GetCategoryBySlugAsync(string slug)
    {
        var category = await _unitOfWork.Categories.GetAsync(c => c.Slug == slug);
        if (category == null) return null;

        var productCount = await _unitOfWork.Products.CountAsync(p => p.CategoryId == category.Id && p.IsActive);

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            Description = category.Description,
            ImageUrl = category.ImageUrl,
            SortOrder = category.SortOrder,
            IsActive = category.IsActive,
            ProductCount = productCount
        };
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request)
    {
        var existingCategory = await _unitOfWork.Categories.GetAsync(c => c.Slug == request.Slug);
        if (existingCategory != null)
        {
            throw new InvalidOperationException("Category with this slug already exists");
        }

        var category = new Category
        {
            Name = request.Name,
            Slug = request.Slug,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            SortOrder = request.SortOrder,
            IsActive = request.IsActive
        };

        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            Description = category.Description,
            ImageUrl = category.ImageUrl,
            SortOrder = category.SortOrder,
            IsActive = category.IsActive,
            ProductCount = 0
        };
    }

    public async Task<CategoryDto?> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null) return null;

        if (!string.IsNullOrWhiteSpace(request.Name))
            category.Name = request.Name;

        if (!string.IsNullOrWhiteSpace(request.Slug))
        {
            var existingCategory = await _unitOfWork.Categories.GetAsync(c => c.Slug == request.Slug && c.Id != id);
            if (existingCategory != null)
            {
                throw new InvalidOperationException("Category with this slug already exists");
            }
            category.Slug = request.Slug;
        }

        if (request.Description != null)
            category.Description = request.Description;

        if (request.ImageUrl != null)
            category.ImageUrl = request.ImageUrl;

        if (request.SortOrder.HasValue)
            category.SortOrder = request.SortOrder.Value;

        if (request.IsActive.HasValue)
            category.IsActive = request.IsActive.Value;

        await _unitOfWork.Categories.UpdateAsync(category);
        await _unitOfWork.SaveChangesAsync();

        var productCount = await _unitOfWork.Products.CountAsync(p => p.CategoryId == id && p.IsActive);

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            Description = category.Description,
            ImageUrl = category.ImageUrl,
            SortOrder = category.SortOrder,
            IsActive = category.IsActive,
            ProductCount = productCount
        };
    }

    public async Task<bool> DeleteCategoryAsync(Guid id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null) return false;

        var hasProducts = await _unitOfWork.Products.ExistsAsync(p => p.CategoryId == id);
        if (hasProducts)
        {
            throw new InvalidOperationException("Cannot delete category with existing products");
        }

        await _unitOfWork.Categories.DeleteAsync(category);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
