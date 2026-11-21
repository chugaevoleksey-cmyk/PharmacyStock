using PharmacyStock.Core.Domain;

namespace PharmacyStock.Core.Abstractions;

/// <summary>Represents the IPricingPolicy type.</summary>
public interface IPricingPolicy
{
    /// <summary>Prices one line with this policy and rounds to two decimal places.</summary>
    /// <param name="line">Immutable medicine line.</param>
    /// <returns>The result of the operation.</returns>
    decimal PriceOf(PrescriptionLine line);
}
