using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

public class CreateSaleRequestValidator : AbstractValidator<CreateSaleRequest>
{
    public CreateSaleRequestValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty()
            .WithMessage("Customer ID is required");
        
        RuleFor(x => x.BranchId).NotEmpty()
            .WithMessage("Branch ID is required");
        
        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("At least one item is required")
            .Must(list => list.Select(i => i.ProductId).Distinct().Count() == list.Count)
            .WithMessage("Duplicate products are not allowed");
        
        RuleForEach(x => x.Items)
            .ChildRules(item =>
            {
                item.RuleFor(i => i.ProductId).NotEmpty()
                    .WithMessage("Product ID is required");
                
                item.RuleFor(i => i.Quantity)
                    .GreaterThan(0)
                    .WithMessage("Quantity must be greater than zero")
                    .LessThanOrEqualTo(20)
                    .WithMessage("Quantity cannot exceed 20 items");
            });
    }
}
