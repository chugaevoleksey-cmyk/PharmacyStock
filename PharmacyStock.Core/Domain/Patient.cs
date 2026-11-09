namespace PharmacyStock.Core.Domain;

/// <summary>Пацієнт аптеки.</summary>
public sealed class Patient
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public DateOnly BirthDate { get; init; }
}