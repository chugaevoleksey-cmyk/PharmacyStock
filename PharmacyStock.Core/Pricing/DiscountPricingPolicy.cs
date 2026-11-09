using PharmacyStock.Core.Abstractions;
using PharmacyStock.Core.Domain;

namespace PharmacyStock.Core.Pricing;

/// <summary>Represents the DiscountPricingPolicy type.</summary>
public sealed class DiscountPricingPolicy : IPricingPolicy
{
    private readonly decimal _rate;

    /// <summary>Initializes a new instance of the <see cref="DiscountPricingPolicy"/> class.</summary>
    /// <param name="rate">The rate argument.</param>
    public DiscountPricingPolicy(decimal rate)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(rate, 0m);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(rate, 0.5m);
        _rate = rate;
    }

    /// <summary>Performs the PriceOf operation.</summary>
    /// <param name="line">The line argument.</param>
    /// <returns>The result of the operation.</returns>
    public decimal PriceOf(PrescriptionLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        return Math.Round(line.Amount * (1m - _rate), 2);
    }
}
