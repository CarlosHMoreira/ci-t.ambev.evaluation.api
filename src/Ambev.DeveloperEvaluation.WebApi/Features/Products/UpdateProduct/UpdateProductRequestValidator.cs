using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.UpdateProduct;

public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(p => p.Id).NotEmpty();
        RuleFor(p => p.Title).NotEmpty().MaximumLength(100);
        RuleFor(p => p.Price).GreaterThanOrEqualTo(0);
        RuleFor(p => p.Description).NotEmpty().MaximumLength(1000);
        RuleFor(p => p.Category).NotEmpty().MaximumLength(100);
        RuleFor(p => p.Image)
            .NotEmpty()
            .MaximumLength(500)
            .Must(BeValidUrl).WithMessage("Image must be a valid absolute HTTP/HTTPS URL");
        RuleFor(p => p.Rating.Rate).InclusiveBetween(0,5);
        RuleFor(p => p.Rating.Count).GreaterThanOrEqualTo(0);
    }

    private static bool BeValidUrl(string url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri)
        && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}
