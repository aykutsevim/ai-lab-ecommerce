using GameVault.Core.DTOs.Categories;
using GameVault.Core.DTOs.Common;
using GameVault.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetCategories()
    {
        try
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(ApiResponse<IEnumerable<CategoryDto>>.SuccessResponse(categories));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<CategoryDto>>.ErrorResponse("An error occurred while retrieving categories", new List<string> { ex.Message }));
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> GetCategoryById(Guid id)
    {
        try
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound(ApiResponse<CategoryDto>.ErrorResponse("Category not found"));
            }

            return Ok(ApiResponse<CategoryDto>.SuccessResponse(category));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<CategoryDto>.ErrorResponse("An error occurred while retrieving the category", new List<string> { ex.Message }));
        }
    }

    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> GetCategoryBySlug(string slug)
    {
        try
        {
            var category = await _categoryService.GetCategoryBySlugAsync(slug);
            if (category == null)
            {
                return NotFound(ApiResponse<CategoryDto>.ErrorResponse("Category not found"));
            }

            return Ok(ApiResponse<CategoryDto>.SuccessResponse(category));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<CategoryDto>.ErrorResponse("An error occurred while retrieving the category", new List<string> { ex.Message }));
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        try
        {
            var category = await _categoryService.CreateCategoryAsync(request);
            return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, ApiResponse<CategoryDto>.SuccessResponse(category, "Category created successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<CategoryDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<CategoryDto>.ErrorResponse("An error occurred while creating the category", new List<string> { ex.Message }));
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> UpdateCategory(Guid id, [FromBody] UpdateCategoryRequest request)
    {
        try
        {
            var category = await _categoryService.UpdateCategoryAsync(id, request);
            if (category == null)
            {
                return NotFound(ApiResponse<CategoryDto>.ErrorResponse("Category not found"));
            }

            return Ok(ApiResponse<CategoryDto>.SuccessResponse(category, "Category updated successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<CategoryDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<CategoryDto>.ErrorResponse("An error occurred while updating the category", new List<string> { ex.Message }));
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteCategory(Guid id)
    {
        try
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Category not found"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Category deleted successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while deleting the category", new List<string> { ex.Message }));
        }
    }
}
