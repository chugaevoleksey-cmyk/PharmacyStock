namespace PharmacyStock.Core.Domain;

/// <summary>Лікарський засіб, який обліковується в аптеці.</summary>
public sealed class Medicine
{
    public string Code { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public int Stock { get; init; }
}