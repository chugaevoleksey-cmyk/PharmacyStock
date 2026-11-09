namespace PharmacyStock.Core.Domain;

/// <summary>Пацієнт аптеки.</summary>
public sealed class Patient
{
    /// <summary>Gets the document or patient identifier.</summary>
    public int Id { get; init; }

    /// <summary>Gets the display name.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Gets the patient date of birth.</summary>
    public DateOnly BirthDate { get; init; }
}
