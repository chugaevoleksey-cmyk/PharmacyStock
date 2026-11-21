namespace PharmacyStock.Core.Documents;

/// <summary>Contains the total and resulting state.</summary>
public sealed record TotalResult
{
    /// <summary>Gets the resulting total.</summary>
    public decimal Total { get; init; }

    /// <summary>Gets the document state.</summary>
    public DocumentStatus Status { get; init; }
}
