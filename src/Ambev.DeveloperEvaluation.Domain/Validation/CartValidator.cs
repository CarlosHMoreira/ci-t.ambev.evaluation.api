using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

/// <summary>
/// Validator for Cart entity
/// </summary>
public class CartValidator : AbstractValidator<Cart>
{
    public CartValidator()
    {
        RuleFor(cart => cart.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(cart => cart.Date)
            .NotEmpty()
            .WithMessage("Date is required");

        RuleFor(cart => cart.Products)
            .NotEmpty()
            .WithMessage("Cart must contain at least one product");

        RuleForEach(cart => cart.Products)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.ProductId)
                    .NotEmpty()
                    .WithMessage("Product ID is required");

                item.RuleFor(x => x.Quantity)
                    .GreaterThan(0)
                    .WithMessage("Quantity must be greater than 0");
            });
    }
}

