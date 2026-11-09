namespace PharmacyStock.Core.Documents;

public sealed record PatientInfo
{
    public required string Name { get; init; }

    public required string Email { get; init; }

    public bool IsRegular { get; init; }
}
