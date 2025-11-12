using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleItemTests
{
    [Fact]
    public void CalculateSaleValue_ComputesAmounts()
    {
        var item = new SaleItem { Quantity = 10, UnitPrice = 10, DiscountPercent = 10.0m};
        item.CalculateSaleValue();
        item.TotalGrossAmount.Should().Be(100);
        item.DiscountValue.Should().Be(10);
        item.TotalNetAmount.Should().Be(90);
    }
}

