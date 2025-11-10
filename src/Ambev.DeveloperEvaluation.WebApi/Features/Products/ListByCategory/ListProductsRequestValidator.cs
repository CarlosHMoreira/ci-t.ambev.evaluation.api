using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.ListByCategory;

public class ListByCategoryRequestValidator : AbstractValidator<ListByCategoryRequest>
{
    public ListByCategoryRequestValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0).When(x => x.Page.HasValue);
        RuleFor(x => x.Size).GreaterThan(0).When(x => x.Size.HasValue);
        RuleFor(x => x.Category).NotEmpty();
    }
}

