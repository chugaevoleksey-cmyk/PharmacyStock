namespace PharmacyStock.Core.Domain;

/// <summary>Одна позиція рецепта з ліками та кількістю.</summary>
public sealed class PrescriptionLine
{
    public string MedicineCode { get; init; } = string.Empty;

    public int Quantity { get; init; }

    public decimal UnitPrice { get; init; }

    public decimal Amount => Quantity * UnitPrice;
}
