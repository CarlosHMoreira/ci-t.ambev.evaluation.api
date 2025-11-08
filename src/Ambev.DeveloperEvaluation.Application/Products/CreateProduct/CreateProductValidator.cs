using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Products.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(p => p.Title).NotEmpty().MaximumLength(100);
        RuleFor(p => p.Price).GreaterThanOrEqualTo(0);
        RuleFor(p => p.Description).NotEmpty().MaximumLength(1000);
        RuleFor(p => p.Category).NotEmpty().MaximumLength(100);
        RuleFor(p => p.Image).NotEmpty().MaximumLength(500);
        RuleFor(p => p.Rate).InclusiveBetween(0,5);
        RuleFor(p => p.Count).GreaterThanOrEqualTo(0);
    }
}