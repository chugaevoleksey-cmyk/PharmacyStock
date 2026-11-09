namespace PharmacyStock.Core.Documents;

public sealed record TotalResult
{
    public decimal Total { get; init; }

    public DocumentStatus Status { get; init; }
}
