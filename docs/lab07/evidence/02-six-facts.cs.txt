using PharmacyStock.Core.Pricing;
using Xunit;

namespace PharmacyStock.Tests;

public sealed class PrescriptionDiscountPolicyTests
{
    [Fact]
    public void DiscountFor_ZeroAmount_ReturnsZero()
    {
        // Arrange
        var policy = new PrescriptionDiscountPolicy();

        // Act
        decimal actual = policy.DiscountFor(0m, false);

        // Assert
        Assert.Equal(0m, actual);
    }

    [Fact]
    public void DiscountFor_RegularPatient_ReturnsFivePercent()
    {
        // Arrange
        var policy = new PrescriptionDiscountPolicy();

        // Act
        decimal actual = policy.DiscountFor(400m, true);

        // Assert
        Assert.Equal(20m, actual);
    }

    [Fact]
    public void DiscountFor_BelowThresholdNewPatient_ReturnsZero()
    {
        // Arrange
        var policy = new PrescriptionDiscountPolicy();

        // Act
        decimal actual = policy.DiscountFor(999.99m, false);

        // Assert
        Assert.Equal(0m, actual);
    }

    [Fact]
    public void DiscountFor_BelowThresholdRegularPatient_ReturnsFifty()
    {
        // Arrange
        var policy = new PrescriptionDiscountPolicy();

        // Act
        decimal actual = policy.DiscountFor(999.99m, true);

        // Assert
        Assert.Equal(50m, actual);
    }

    [Fact]
    public void DiscountFor_AtThresholdNewPatient_ReturnsHundred()
    {
        // Arrange
        var policy = new PrescriptionDiscountPolicy();

        // Act
        decimal actual = policy.DiscountFor(1000m, false);

        // Assert
        Assert.Equal(100m, actual);
    }

    [Fact]
    public void DiscountFor_AtThresholdRegularPatient_ReturnsHundredFifty()
    {
        // Arrange
        var policy = new PrescriptionDiscountPolicy();

        // Act
        decimal actual = policy.DiscountFor(1000m, true);

        // Assert
        Assert.Equal(150m, actual);
    }

}
