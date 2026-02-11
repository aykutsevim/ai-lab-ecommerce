namespace GameVault.Core.DTOs.Categories;

public class UpdateCategoryRequest
{
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int? SortOrder { get; set; }
    public bool? IsActive { get; set; }
}
