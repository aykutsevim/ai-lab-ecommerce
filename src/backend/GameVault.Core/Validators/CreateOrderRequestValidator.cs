using FluentValidation;
using GameVault.Core.DTOs.Orders;

namespace GameVault.Core.Validators;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Order must contain at least one item")
            .Must(items => items.Count <= 50).WithMessage("Order cannot contain more than 50 items");

        RuleForEach(x => x.Items)
            .SetValidator(new CreateOrderItemRequestValidator());

        When(x => !string.IsNullOrEmpty(x.ShippingAddress), () =>
        {
            RuleFor(x => x.ShippingAddress)
                .MaximumLength(1000).WithMessage("Shipping address must not exceed 1000 characters");
        });

        When(x => !string.IsNullOrEmpty(x.Notes), () =>
        {
            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notes must not exceed 500 characters");
        });
    }
}
