namespace GameVault.Core.DTOs.Products;

public class ProductFilterRequest
{
    public Guid? CategoryId { get; set; }
    public string? Brand { get; set; }
    public string? Subcategory { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public decimal? MinRating { get; set; }
    public bool? InStock { get; set; }
    public string? Search { get; set; }
    public List<string>? Tags { get; set; }

    public string SortBy { get; set; } = "createdAt";
    public string SortOrder { get; set; } = "desc";
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
