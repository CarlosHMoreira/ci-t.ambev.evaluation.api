using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class SaleItemValidator : AbstractValidator<SaleItem>
{
    public SaleItemValidator()
    {
        RuleFor(i => i.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required");

        RuleFor(i => i.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero")
            .LessThanOrEqualTo(20)
            .WithMessage("Quantity cannot exceed 20 items");

        RuleFor(i => i.UnitPrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Unit price cannot be negative");

        RuleFor(i => i.DiscountPercent)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Discount percent cannot be negative")
            .LessThanOrEqualTo(100)
            .WithMessage("Discount percent cannot exceed 100%");

        RuleFor(i => i.DiscountValue)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Discount value cannot be negative");

        RuleFor(i => i.TotalGrossAmount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Total gross amount cannot be negative");

        RuleFor(i => i.TotalNetAmount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Total net amount cannot be negative");
    }
}

