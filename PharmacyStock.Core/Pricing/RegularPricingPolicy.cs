using PharmacyStock.Core.Abstractions;
using PharmacyStock.Core.Domain;

namespace PharmacyStock.Core.Pricing;

/// <summary>Represents the RegularPricingPolicy type.</summary>
public sealed class RegularPricingPolicy : IPricingPolicy
{
    /// <summary>Performs the PriceOf operation.</summary>
    /// <param name="line">The line argument.</param>
    /// <returns>The result of the operation.</returns>
    public decimal PriceOf(PrescriptionLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        return Math.Round(line.Amount, 2);
    }
}
