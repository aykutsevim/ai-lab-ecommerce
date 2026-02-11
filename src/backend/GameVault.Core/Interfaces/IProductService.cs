using GameVault.Core.DTOs.Common;
using GameVault.Core.DTOs.Products;

namespace GameVault.Core.Interfaces;

public interface IProductService
{
    Task<PagedResult<ProductListDto>> GetProductsAsync(ProductFilterRequest filter);
    Task<ProductDto?> GetProductByIdAsync(Guid id);
    Task<ProductDto?> GetProductBySlugAsync(string slug);
    Task<IEnumerable<string>> GetBrandsAsync();
    Task<IEnumerable<string>> GetTagsAsync();
}
