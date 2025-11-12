using Ambev.DeveloperEvaluation.Domain.Specifications;
using Ambev.DeveloperEvaluation.Unit.Domain.Specifications.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Specifications;

public class DiscountEligibilityByQuantitySpecificationTests
{
    [Theory(DisplayName = "Given quantity When checking 10% discount eligibility Then returns expected result")]
    [InlineData(1, false)]
    [InlineData(2, false)]
    [InlineData(3, false)]
    [InlineData(4, true)]
    [InlineData(5, true)]
    [InlineData(6, true)]
    [InlineData(7, true)]
    [InlineData(8, true)]
    [InlineData(9, true)]
    [InlineData(10, false)]
    [InlineData(15, false)]
    [InlineData(20, false)]
    [InlineData(25, false)]
    public void IsSatisfiedBy_TenPercentSpecification_ValidatesQuantityRange(int quantity, bool expectedResult)
    {
        var item = DiscountEligibilitySpecificationTestData.GenerateSaleItem(quantity);
        var specification = new DiscountElegibilityTenPercentSpecification();

        var result = specification.IsSatisfiedBy(item);

        result.Should().Be(expectedResult);
    }

    [Theory(DisplayName = "Given quantity When checking 20% discount eligibility Then returns expected result")]
    [InlineData(1, false)]
    [InlineData(4, false)]
    [InlineData(9, false)]
    [InlineData(10, true)]
    [InlineData(11, true)]
    [InlineData(12, true)]
    [InlineData(15, true)]
    [InlineData(18, true)]
    [InlineData(19, true)]
    [InlineData(20, true)]
    [InlineData(21, false)]
    [InlineData(25, false)]
    [InlineData(100, false)]
    public void IsSatisfiedBy_TwentyPercentSpecification_ValidatesQuantityRange(int quantity, bool expectedResult)
    {
        var item = DiscountEligibilitySpecificationTestData.GenerateSaleItem(quantity);
        var specification = new DiscountElegibilityTwentyPercentSpecification();

        var result = specification.IsSatisfiedBy(item);

        result.Should().Be(expectedResult);
    }

    [Fact(DisplayName = "Given quantity exactly at lower bound of 10% range When checking Then returns true")]
    public void IsSatisfiedBy_TenPercent_LowerBound_ReturnsTrue()
    {
        var item = DiscountEligibilitySpecificationTestData.GenerateSaleItem(4);
        var specification = new DiscountElegibilityTenPercentSpecification();

        var result = specification.IsSatisfiedBy(item);

        result.Should().BeTrue();
    }

    [Fact(DisplayName = "Given quantity exactly at upper bound of 10% range When checking Then returns true")]
    public void IsSatisfiedBy_TenPercent_UpperBound_ReturnsTrue()
    {
        var item = DiscountEligibilitySpecificationTestData.GenerateSaleItem(9);
        var specification = new DiscountElegibilityTenPercentSpecification();

        var result = specification.IsSatisfiedBy(item);

        result.Should().BeTrue();
    }

    [Fact(DisplayName = "Given quantity exactly at lower bound of 20% range When checking Then returns true")]
    public void IsSatisfiedBy_TwentyPercent_LowerBound_ReturnsTrue()
    {
        var item = DiscountEligibilitySpecificationTestData.GenerateSaleItem(10);
        var specification = new DiscountElegibilityTwentyPercentSpecification();

        var result = specification.IsSatisfiedBy(item);

        result.Should().BeTrue();
    }

    [Fact(DisplayName = "Given quantity exactly at upper bound of 20% range When checking Then returns true")]
    public void IsSatisfiedBy_TwentyPercent_UpperBound_ReturnsTrue()
    {
        var item = DiscountEligibilitySpecificationTestData.GenerateSaleItem(20);
        var specification = new DiscountElegibilityTwentyPercentSpecification();

        var result = specification.IsSatisfiedBy(item);

        result.Should().BeTrue();
    }

    [Fact(DisplayName = "Given quantity just below 10% range When checking Then returns false")]
    public void IsSatisfiedBy_TenPercent_BelowRange_ReturnsFalse()
    {
        var item = DiscountEligibilitySpecificationTestData.GenerateSaleItem(3);
        var specification = new DiscountElegibilityTenPercentSpecification();

        var result = specification.IsSatisfiedBy(item);

        result.Should().BeFalse();
    }

    [Fact(DisplayName = "Given quantity just above 10% range When checking Then returns false")]
    public void IsSatisfiedBy_TenPercent_AboveRange_ReturnsFalse()
    {
        var item = DiscountEligibilitySpecificationTestData.GenerateSaleItem(10);
        var specification = new DiscountElegibilityTenPercentSpecification();

        var result = specification.IsSatisfiedBy(item);

        result.Should().BeFalse();
    }

    [Fact(DisplayName = "Given quantity just below 20% range When checking Then returns false")]
    public void IsSatisfiedBy_TwentyPercent_BelowRange_ReturnsFalse()
    {
        var item = DiscountEligibilitySpecificationTestData.GenerateSaleItem(9);
        var specification = new DiscountElegibilityTwentyPercentSpecification();

        var result = specification.IsSatisfiedBy(item);

        result.Should().BeFalse();
    }

    [Fact(DisplayName = "Given quantity just above 20% range When checking Then returns false")]
    public void IsSatisfiedBy_TwentyPercent_AboveRange_ReturnsFalse()
    {
        var item = DiscountEligibilitySpecificationTestData.GenerateSaleItem(21);
        var specification = new DiscountElegibilityTwentyPercentSpecification();

        var result = specification.IsSatisfiedBy(item);

        result.Should().BeFalse();
    }

    [Fact(DisplayName = "Given zero quantity When checking 10% eligibility Then returns false")]
    public void IsSatisfiedBy_TenPercent_ZeroQuantity_ReturnsFalse()
    {
        var item = DiscountEligibilitySpecificationTestData.GenerateSaleItem(0);
        var specification = new DiscountElegibilityTenPercentSpecification();

        var result = specification.IsSatisfiedBy(item);

        result.Should().BeFalse();
    }

    [Fact(DisplayName = "Given zero quantity When checking 20% eligibility Then returns false")]
    public void IsSatisfiedBy_TwentyPercent_ZeroQuantity_ReturnsFalse()
    {
        var item = DiscountEligibilitySpecificationTestData.GenerateSaleItem(0);
        var specification = new DiscountElegibilityTwentyPercentSpecification();

        var result = specification.IsSatisfiedBy(item);

        result.Should().BeFalse();
    }

    [Fact(DisplayName = "Given negative quantity When checking 10% eligibility Then returns false")]
    public void IsSatisfiedBy_TenPercent_NegativeQuantity_ReturnsFalse()
    {
        var item = DiscountEligibilitySpecificationTestData.GenerateSaleItem(-1);
        var specification = new DiscountElegibilityTenPercentSpecification();

        var result = specification.IsSatisfiedBy(item);

        result.Should().BeFalse();
    }

    [Fact(DisplayName = "Given negative quantity When checking 20% eligibility Then returns false")]
    public void IsSatisfiedBy_TwentyPercent_NegativeQuantity_ReturnsFalse()
    {
        var item = DiscountEligibilitySpecificationTestData.GenerateSaleItem(-5);
        var specification = new DiscountElegibilityTwentyPercentSpecification();

        var result = specification.IsSatisfiedBy(item);

        result.Should().BeFalse();
    }

    [Fact(DisplayName = "Given item with cancelled status When checking eligibility Then specification only checks quantity")]
    public void IsSatisfiedBy_CancelledItem_ChecksOnlyQuantity()
    {
        var item = DiscountEligibilitySpecificationTestData.GenerateSaleItem(5);
        item.IsCancelled = true;
        var specification = new DiscountElegibilityTenPercentSpecification();

        var result = specification.IsSatisfiedBy(item);

        result.Should().BeTrue();
    }

    [Fact(DisplayName = "Given item with existing discount When checking eligibility Then specification only checks quantity")]
    public void IsSatisfiedBy_ItemWithExistingDiscount_ChecksOnlyQuantity()
    {
        var item = DiscountEligibilitySpecificationTestData.GenerateSaleItemWithDiscount(5, 15m);
        var specification = new DiscountElegibilityTenPercentSpecification();

        var result = specification.IsSatisfiedBy(item);

        result.Should().BeTrue();
    }

    [Fact(DisplayName = "Given multiple items with different quantities When checking Then validates each independently")]
    public void IsSatisfiedBy_MultipleItems_ValidatesIndependently()
    {
        var item1 = DiscountEligibilitySpecificationTestData.GenerateSaleItem(3);
        var item2 = DiscountEligibilitySpecificationTestData.GenerateSaleItem(5);
        var item3 = DiscountEligibilitySpecificationTestData.GenerateSaleItem(15);
        var spec10 = new DiscountElegibilityTenPercentSpecification();
        var spec20 = new DiscountElegibilityTwentyPercentSpecification();

        var result1For10Percent = spec10.IsSatisfiedBy(item1);
        var result2For10Percent = spec10.IsSatisfiedBy(item2);
        var result3For20Percent = spec20.IsSatisfiedBy(item3);

        result1For10Percent.Should().BeFalse();
        result2For10Percent.Should().BeTrue();
        result3For20Percent.Should().BeTrue();
    }
}

