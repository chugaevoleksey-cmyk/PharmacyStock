using PharmacyStock.Core.Abstractions;
using PharmacyStock.Core.Domain;

namespace PharmacyStock.Core.Pricing;

/// <summary>Represents the VipPricingPolicy type.</summary>
public sealed class VipPricingPolicy : IPricingPolicy
{
    private const decimal PriceMultiplier = 0.90m;

    /// <summary>Prices one line with this policy and rounds to two decimal places.</summary>
    /// <param name="line">Immutable medicine line.</param>
    /// <returns>The result of the operation.</returns>
    public decimal PriceOf(PrescriptionLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        return Math.Round(line.Amount * PriceMultiplier, 2);
    }
}
