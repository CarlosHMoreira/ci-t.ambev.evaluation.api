using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Specifications;

public abstract class DiscountEligibilityByQuantitySpecification(int min, int max) : ISpecification<SaleItem>
{
    public bool IsSatisfiedBy(SaleItem item) => item.Quantity >= min && item.Quantity <= max;
}

public sealed class DiscountElegibilityTenPercentSpecification() : DiscountEligibilityByQuantitySpecification(4, 9);
public sealed class DiscountElegibilityTwentyPercentSpecification() : DiscountEligibilityByQuantitySpecification(10, 20);
