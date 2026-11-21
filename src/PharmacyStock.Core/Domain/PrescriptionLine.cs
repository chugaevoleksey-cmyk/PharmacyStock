namespace PharmacyStock.Core.Domain;

/// <summary>Represents the PrescriptionLine type.</summary>
public sealed record PrescriptionLine
{
    /// <summary>Initializes a new instance of the <see cref="PrescriptionLine"/> class.</summary>
    /// <param name="medicineCode">Nonblank medicine code.</param>
    /// <param name="quantity">Positive number of units.</param>
    /// <param name="unitPrice">Nonnegative price fixed for this line.</param>
    public PrescriptionLine(string medicineCode, int quantity, decimal unitPrice)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(medicineCode);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        ArgumentOutOfRangeException.ThrowIfLessThan(unitPrice, 0m);

        MedicineCode = medicineCode;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    /// <summary>Gets the MedicineCode value.</summary>
    public string MedicineCode { get; }

    /// <summary>Gets the Quantity value.</summary>
    public int Quantity { get; }

    /// <summary>Gets the UnitPrice value.</summary>
    public decimal UnitPrice { get; }

    /// <summary>Gets the Amount value.</summary>
    public decimal Amount => Quantity * UnitPrice;
}
