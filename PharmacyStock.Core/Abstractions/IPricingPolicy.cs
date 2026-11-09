using PharmacyStock.Core.Domain;

namespace PharmacyStock.Core.Abstractions;

/// <summary>Represents the IPricingPolicy type.</summary>
public interface IPricingPolicy
{
    /// <summary>Performs the PriceOf operation.</summary>
    /// <param name="line">The line argument.</param>
    /// <returns>The result of the operation.</returns>
    decimal PriceOf(PrescriptionLine line);
}
