using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(p => p.Title).NotEmpty().MaximumLength(100);
        RuleFor(p => p.Price).GreaterThanOrEqualTo(0);
        RuleFor(p => p.Description).NotEmpty().MaximumLength(1000);
        RuleFor(p => p.Category).NotEmpty().MaximumLength(100);
        RuleFor(p => p.Image).NotEmpty().MaximumLength(500);
        RuleFor(p => p.Rating.Rate).InclusiveBetween(0,5);
        RuleFor(p => p.Rating.Count).GreaterThanOrEqualTo(0);
    }
}

