using PharmacyStock.Core.Abstractions;
using PharmacyStock.Core.Domain;

namespace PharmacyStock.Core.Pricing;

/// <summary>Represents the PricingTotals type.</summary>
public static class PricingTotals
{
    private const decimal DiscountThreshold = 1000m;
    private const decimal FixedDiscount = 50m;

    /// <summary>Performs the TotalOf operation.</summary>
    /// <param name="lines">The lines argument.</param>
    /// <param name="pricing">The pricing argument.</param>
    /// <returns>The result of the operation.</returns>
    public static decimal TotalOf(IReadOnlyList<PrescriptionLine> lines, IPricingPolicy pricing)
    {
        ArgumentNullException.ThrowIfNull(lines);
        ArgumentNullException.ThrowIfNull(pricing);
        decimal total = 0m;
        foreach (PrescriptionLine line in lines)
        {
            ArgumentNullException.ThrowIfNull(line);
            decimal price = pricing.PriceOf(line);
            ArgumentOutOfRangeException.ThrowIfLessThan(price, 0m);
            total += price;
        }

        return total > DiscountThreshold ? total - FixedDiscount : total;
    }
}
