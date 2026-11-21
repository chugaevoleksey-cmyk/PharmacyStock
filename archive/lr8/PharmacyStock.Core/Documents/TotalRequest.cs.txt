namespace PharmacyStock.Core.Documents;

/// <summary>Contains the calculation input.</summary>
public sealed record TotalRequest
{
    /// <summary>Gets the positive document identifier.</summary>
    public int DocumentId { get; init; }

    /// <summary>Gets the patient details.</summary>
    public required PatientInfo Patient { get; init; }

    /// <summary>Gets the medicine lines.</summary>
    public required IReadOnlyList<PrescriptionLine> Lines { get; init; }

    /// <summary>Gets the creation date.</summary>
    public DateOnly CreatedAt { get; init; }

    /// <summary>Gets the optional coupon.</summary>
    public string? CouponCode { get; init; }

    /// <summary>Gets the document state.</summary>
    public DocumentStatus Status { get; init; }

    /// <summary>Gets the delivery price.</summary>
    public decimal DeliveryPrice { get; init; }
}
