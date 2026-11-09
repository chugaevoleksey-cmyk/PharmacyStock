namespace PharmacyStock.Core.Domain;

/// <summary>Одна позиція рецепта з ліками та кількістю.</summary>
public sealed class PrescriptionLine
{
    /// <summary>Gets the medicine code for the line.</summary>
    public string MedicineCode { get; init; } = string.Empty;

    /// <summary>Gets the number of units.</summary>
    public int Quantity { get; init; }

    /// <summary>Gets the unit price.</summary>
    public decimal UnitPrice { get; init; }

    /// <summary>Gets the unit price.</summary>
    public decimal Amount => Quantity * UnitPrice;
}
