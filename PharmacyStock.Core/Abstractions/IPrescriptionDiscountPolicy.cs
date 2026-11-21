namespace PharmacyStock.Core.Abstractions;

/// <summary>Calculates a promotional discount on a complete prescription.</summary>
public interface IPrescriptionDiscountPolicy
{
    /// <summary>Calculates the discount amount.</summary>
    /// <param name="amount">Prescription amount before the promotion.</param>
    /// <param name="isRegular">Whether the patient is regular.</param>
    /// <returns>The discount amount.</returns>
    decimal DiscountFor(decimal amount, bool isRegular);
}
