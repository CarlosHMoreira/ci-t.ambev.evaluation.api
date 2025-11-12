using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleItemTests
{
    [Theory]
    [InlineData(1, 100, 0)]
    [InlineData(3, 50, 0)]
    [InlineData(4, 10, 10)]
    [InlineData(9, 10, 10)]
    [InlineData(10, 10, 20)]
    [InlineData(20, 10, 20)]
    public void ApplyRules_DiscountPercentBasedOnQuantity(int quantity, decimal unitPrice, decimal expectedPercent)
    {
        var item = new SaleItem { Quantity = quantity, UnitPrice = unitPrice };
        item.ApplyRules();
        item.DiscountPercent.Should().Be(expectedPercent);
    }

    [Fact]
    public void ApplyRules_ComputesAmounts()
    {
        var item = new SaleItem { Quantity = 10, UnitPrice = 5 }; // 20% discount
        item.ApplyRules();
        item.TotalGrossAmount.Should().Be(50);
        item.DiscountValue.Should().Be(10); // 20% of 50
        item.TotalNetAmount.Should().Be(40);
    }
}

