using GameVault.Core.DTOs.Common;
using GameVault.Core.DTOs.Products;
using GameVault.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GameVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ProductListDto>>>> GetProducts([FromQuery] ProductFilterRequest filter)
    {
        try
        {
            var products = await _productService.GetProductsAsync(filter);
            return Ok(ApiResponse<PagedResult<ProductListDto>>.SuccessResponse(products));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PagedResult<ProductListDto>>.ErrorResponse("An error occurred while retrieving products", new List<string> { ex.Message }));
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetProductById(Guid id)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound(ApiResponse<ProductDto>.ErrorResponse("Product not found"));
            }

            return Ok(ApiResponse<ProductDto>.SuccessResponse(product));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ProductDto>.ErrorResponse("An error occurred while retrieving the product", new List<string> { ex.Message }));
        }
    }

    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetProductBySlug(string slug)
    {
        try
        {
            var product = await _productService.GetProductBySlugAsync(slug);
            if (product == null)
            {
                return NotFound(ApiResponse<ProductDto>.ErrorResponse("Product not found"));
            }

            return Ok(ApiResponse<ProductDto>.SuccessResponse(product));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ProductDto>.ErrorResponse("An error occurred while retrieving the product", new List<string> { ex.Message }));
        }
    }

    [HttpGet("brands")]
    public async Task<ActionResult<ApiResponse<IEnumerable<string>>>> GetBrands()
    {
        try
        {
            var brands = await _productService.GetBrandsAsync();
            return Ok(ApiResponse<IEnumerable<string>>.SuccessResponse(brands));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<string>>.ErrorResponse("An error occurred while retrieving brands", new List<string> { ex.Message }));
        }
    }

    [HttpGet("tags")]
    public async Task<ActionResult<ApiResponse<IEnumerable<string>>>> GetTags()
    {
        try
        {
            var tags = await _productService.GetTagsAsync();
            return Ok(ApiResponse<IEnumerable<string>>.SuccessResponse(tags));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<string>>.ErrorResponse("An error occurred while retrieving tags", new List<string> { ex.Message }));
        }
    }
}
