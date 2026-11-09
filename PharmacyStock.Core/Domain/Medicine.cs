namespace PharmacyStock.Core.Domain;

/// <summary>Лікарський засіб, який обліковується в аптеці.</summary>
public sealed class Medicine
{
    /// <summary>Gets the medicine code.</summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>Gets the display name.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Gets the price per unit.</summary>
    public decimal Price { get; init; }

    /// <summary>Gets the available stock.</summary>
    public int Stock { get; init; }
}
