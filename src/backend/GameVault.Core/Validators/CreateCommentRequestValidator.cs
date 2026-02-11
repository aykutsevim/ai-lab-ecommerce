using FluentValidation;
using GameVault.Core.DTOs.Comments;

namespace GameVault.Core.Validators;

public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest>
{
    public CreateCommentRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required");

        When(x => !string.IsNullOrEmpty(x.Title), () =>
        {
            RuleFor(x => x.Title)
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters");
        });

        RuleFor(x => x.Body)
            .NotEmpty().WithMessage("Comment body is required")
            .MinimumLength(10).WithMessage("Comment must be at least 10 characters")
            .MaximumLength(2000).WithMessage("Comment must not exceed 2000 characters");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5");
    }
}
