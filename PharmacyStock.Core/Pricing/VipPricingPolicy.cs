using PharmacyStock.Core.Abstractions;
using PharmacyStock.Core.Domain;

namespace PharmacyStock.Core.Pricing;

/// <summary>Represents the VipPricingPolicy type.</summary>
public sealed class VipPricingPolicy : IPricingPolicy
{
    private const decimal PriceMultiplier = 0.90m;

    /// <summary>Performs the PriceOf operation.</summary>
    /// <param name="line">The line argument.</param>
    /// <returns>The result of the operation.</returns>
    public decimal PriceOf(PrescriptionLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        return Math.Round(line.Amount * PriceMultiplier, 2);
    }
}
