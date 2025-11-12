using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Specifications;

public class MaxQuantitySpecification : ISpecification<SaleItem>
{
    public bool IsSatisfiedBy(SaleItem item) => item.Quantity <= 20;
}

