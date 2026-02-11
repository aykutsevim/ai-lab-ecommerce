using FluentValidation;
using GameVault.Core.DTOs.Products;

namespace GameVault.Core.Validators;

public class ProductFilterRequestValidator : AbstractValidator<ProductFilterRequest>
{
    private readonly string[] _allowedSortFields = new[]
    {
        "name", "price", "rating", "createdAt", "brand"
    };

    private readonly string[] _allowedSortOrders = new[] { "asc", "desc" };

    public ProductFilterRequestValidator()
    {
        When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue, () =>
        {
            RuleFor(x => x.MinPrice)
                .LessThanOrEqualTo(x => x.MaxPrice)
                .WithMessage("Minimum price must be less than or equal to maximum price");
        });

        When(x => x.MinRating.HasValue, () =>
        {
            RuleFor(x => x.MinRating)
                .InclusiveBetween(0, 5).WithMessage("Rating must be between 0 and 5");
        });

        RuleFor(x => x.SortBy)
            .Must(value => _allowedSortFields.Contains(value.ToLower()))
            .WithMessage($"Sort field must be one of: {string.Join(", ", _allowedSortFields)}");

        RuleFor(x => x.SortOrder)
            .Must(value => _allowedSortOrders.Contains(value.ToLower()))
            .WithMessage("Sort order must be 'asc' or 'desc'");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100");
    }
}
