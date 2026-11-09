namespace PharmacyStock.Core.Domain;

/// <summary>Стан рецепта в аптеці.</summary>
public enum PrescriptionStatus
{
    /// <summary>The New status.</summary>
    New = 0,
    /// <summary>The Approved status.</summary>
    Approved = 1,
    /// <summary>The Dispensed status.</summary>
    Dispensed = 2,
    /// <summary>The Cancelled status.</summary>
    Cancelled = 3,
    /// <summary>The Expired status.</summary>
    Expired = 4,
}
