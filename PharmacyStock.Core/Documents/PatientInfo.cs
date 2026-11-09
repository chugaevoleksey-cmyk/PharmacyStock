namespace PharmacyStock.Core.Documents;

/// <summary>Contains patient details for a calculation.</summary>
public sealed record PatientInfo
{
    /// <summary>Gets the patient name.</summary>
    public required string Name { get; init; }

    /// <summary>Gets the patient email.</summary>
    public required string Email { get; init; }

    /// <summary>Gets a value indicating whether the patient is regular.</summary>
    public bool IsRegular { get; init; }
}
