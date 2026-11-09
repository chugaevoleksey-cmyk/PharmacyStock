namespace PharmacyStock.Core.Documents;

public sealed record PrescriptionLine
{
    public required string MedicineCode { get; init; }

    public int Quantity { get; init; }

    public decimal UnitPrice { get; init; }
}
