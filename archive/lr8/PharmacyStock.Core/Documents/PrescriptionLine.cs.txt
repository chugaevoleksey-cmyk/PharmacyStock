namespace PharmacyStock.Core.Documents;

/// <summary>Represents one medicine line.</summary>
public sealed record PrescriptionLine
{
    /// <summary>Gets the medicine identifier.</summary>
    public required string MedicineCode { get; init; }

    /// <summary>Gets the positive number of units.</summary>
    public int Quantity { get; init; }

    /// <summary>Gets the nonnegative price per unit.</summary>
    public decimal UnitPrice { get; init; }
}
