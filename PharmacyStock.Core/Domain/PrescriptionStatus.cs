namespace PharmacyStock.Core.Domain;

/// <summary>Стан рецепта в аптеці.</summary>
public enum PrescriptionStatus
{
    New = 0,
    Approved = 1,
    Dispensed = 2,
    Cancelled = 3,
    Expired = 4,
}
