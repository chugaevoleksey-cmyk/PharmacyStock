namespace PharmacyStock.Core.Documents;

public sealed record TotalRequest
{
    public int DocumentId { get; init; }

    public required PatientInfo Patient { get; init; }

    public required IReadOnlyList<PrescriptionLine> Lines { get; init; }

    public DateOnly CreatedAt { get; init; }

    public string? CouponCode { get; init; }

    public DocumentStatus Status { get; init; }

    public decimal DeliveryPrice { get; init; }
}
