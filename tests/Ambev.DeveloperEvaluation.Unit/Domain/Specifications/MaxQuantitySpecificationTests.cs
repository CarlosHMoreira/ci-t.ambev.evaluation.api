using Ambev.DeveloperEvaluation.Domain.Specifications;
using Ambev.DeveloperEvaluation.Unit.Domain.Specifications.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Specifications;

public class MaxQuantitySpecificationTests
{
    [Theory(DisplayName = "Given quantity When checking max quantity specification Then returns expected result")]
    [InlineData(0, true)]
    [InlineData(1, true)]
    [InlineData(5, true)]
    [InlineData(10, true)]
    [InlineData(15, true)]
    [InlineData(19, true)]
    [InlineData(20, true)]
    [InlineData(21, false)]
    [InlineData(25, false)]
    [InlineData(50, false)]
    [InlineData(100, false)]
    public void IsSatisfiedBy_ValidatesMaxQuantity(int quantity, bool expectedResult)
    {
        var item = DiscountEligibilitySpecificationTestData.GenerateSaleItem(quantity);
        var specification = new MaxQuantitySpecification();

        var result = specification.IsSatisfiedBy(item);

        result.Should().Be(expectedResult);
    }

    [Fact(DisplayName = "Given quantity exactly at limit When checking Then returns true")]
    public void IsSatisfiedBy_ExactlyAtLimit_ReturnsTrue()
    {
        var item = DiscountEligibilitySpecificationTestData.GenerateSaleItem(20);
        var specification = new MaxQuantitySpecification();

        var result = specification.IsSatisfiedBy(item);

        result.Should().BeTrue();
    }

    [Fact(DisplayName = "Given quantity just above limit When checking Then returns false")]
    public void IsSatisfiedBy_JustAboveLimit_ReturnsFalse()
    {
        var item = DiscountEligibilitySpecificationTestData.GenerateSaleItem(21);
        var specification = new MaxQuantitySpecification();

        var result = specification.IsSatisfiedBy(item);

        result.Should().BeFalse();
    }

    [Fact(DisplayName = "Given quantity just below limit When checking Then returns true")]
    public void IsSatisfiedBy_JustBelowLimit_ReturnsTrue()
    {
        var item = DiscountEligibilitySpecificationTestData.GenerateSaleItem(19);
        var specification = new MaxQuantitySpecification();

        var result = specification.IsSatisfiedBy(item);

        result.Should().BeTrue();
    }

    [Fact(DisplayName = "Given zero quantity When checking Then returns true")]
    public void IsSatisfiedBy_ZeroQuantity_ReturnsTrue()
    {
        var item = DiscountEligibilitySpecificationTestData.GenerateSaleItem(0);
        var specification = new MaxQuantitySpecification();

        var result = specification.IsSatisfiedBy(item);

        result.Should().BeTrue();
    }

    [Fact(DisplayName = "Given negative quantity When checking Then returns true")]
    public void IsSatisfiedBy_NegativeQuantity_ReturnsTrue()
    {
        var item = DiscountEligibilitySpecificationTestData.GenerateSaleItem(-1);
        var specification = new MaxQuantitySpecification();

        var result = specification.IsSatisfiedBy(item);

        result.Should().BeTrue();
    }

    [Fact(DisplayName = "Given cancelled item within limit When checking Then returns true")]
    public void IsSatisfiedBy_CancelledItemWithinLimit_ReturnsTrue()
    {
        var item = DiscountEligibilitySpecificationTestData.GenerateSaleItem(15);
        item.IsCancelled = true;
        var specification = new MaxQuantitySpecification();

        var result = specification.IsSatisfiedBy(item);

        result.Should().BeTrue();
    }

    [Fact(DisplayName = "Given cancelled item above limit When checking Then returns false")]
    public void IsSatisfiedBy_CancelledItemAboveLimit_ReturnsFalse()
    {
        var item = DiscountEligibilitySpecificationTestData.GenerateSaleItem(25);
        item.IsCancelled = true;
        var specification = new MaxQuantitySpecification();

        var result = specification.IsSatisfiedBy(item);

        result.Should().BeFalse();
    }

    [Fact(DisplayName = "Given item with existing discount within limit When checking Then returns true")]
    public void IsSatisfiedBy_ItemWithDiscountWithinLimit_ReturnsTrue()
    {
        var item = DiscountEligibilitySpecificationTestData.GenerateSaleItemWithDiscount(10, 20m);
        var specification = new MaxQuantitySpecification();

        var result = specification.IsSatisfiedBy(item);

        result.Should().BeTrue();
    }

    [Fact(DisplayName = "Given multiple items with different quantities When checking Then validates each independently")]
    public void IsSatisfiedBy_MultipleItems_ValidatesIndependently()
    {
        var item1 = DiscountEligibilitySpecificationTestData.GenerateSaleItem(10);
        var item2 = DiscountEligibilitySpecificationTestData.GenerateSaleItem(20);
        var item3 = DiscountEligibilitySpecificationTestData.GenerateSaleItem(25);
        var specification = new MaxQuantitySpecification();

        var result1 = specification.IsSatisfiedBy(item1);
        var result2 = specification.IsSatisfiedBy(item2);
        var result3 = specification.IsSatisfiedBy(item3);

        result1.Should().BeTrue();
        result2.Should().BeTrue();
        result3.Should().BeFalse();
    }
}

