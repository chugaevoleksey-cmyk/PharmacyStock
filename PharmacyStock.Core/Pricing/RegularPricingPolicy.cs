using PharmacyStock.Core.Abstractions;
using PharmacyStock.Core.Domain;

namespace PharmacyStock.Core.Pricing;

/// <summary>Represents the RegularPricingPolicy type.</summary>
public sealed class RegularPricingPolicy : IPricingPolicy
{
    /// <summary>Prices one line with this policy and rounds to two decimal places.</summary>
    /// <param name="line">Immutable medicine line.</param>
    /// <returns>The result of the operation.</returns>
    public decimal PriceOf(PrescriptionLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        return Math.Round(line.Amount, 2);
    }
}
