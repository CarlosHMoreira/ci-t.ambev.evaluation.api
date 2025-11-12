using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Specifications.TestData;

public static class DiscountEligibilitySpecificationTestData
{
    public static SaleItem GenerateSaleItem(int quantity)
    {
        return new SaleItem
        {
            ProductId = Guid.NewGuid(),
            ProductTitle = "Test Product",
            Quantity = quantity,
            UnitPrice = 100.00m,
            DiscountPercent = 0m,
            DiscountValue = 0m,
            TotalGrossAmount = quantity * 100.00m,
            TotalNetAmount = quantity * 100.00m,
            IsCancelled = false
        };
    }

    public static SaleItem GenerateSaleItemWithDiscount(int quantity, decimal discountPercent)
    {
        var grossAmount = quantity * 100.00m;
        var discountValue = (grossAmount * discountPercent) / 100m;
        var netAmount = grossAmount - discountValue;

        return new SaleItem
        {
            ProductId = Guid.NewGuid(),
            ProductTitle = "Test Product",
            Quantity = quantity,
            UnitPrice = 100.00m,
            DiscountPercent = discountPercent,
            DiscountValue = discountValue,
            TotalGrossAmount = grossAmount,
            TotalNetAmount = netAmount,
            IsCancelled = false
        };
    }
}

