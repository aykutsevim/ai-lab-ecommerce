using FluentValidation;
using GameVault.Core.DTOs.Comments;

namespace GameVault.Core.Validators;

public class UpdateCommentRequestValidator : AbstractValidator<UpdateCommentRequest>
{
    public UpdateCommentRequestValidator()
    {
        When(x => !string.IsNullOrEmpty(x.Title), () =>
        {
            RuleFor(x => x.Title)
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters");
        });

        When(x => !string.IsNullOrEmpty(x.Body), () =>
        {
            RuleFor(x => x.Body)
                .MinimumLength(10).WithMessage("Comment must be at least 10 characters")
                .MaximumLength(2000).WithMessage("Comment must not exceed 2000 characters");
        });

        When(x => x.Rating.HasValue, () =>
        {
            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5");
        });
    }
}
