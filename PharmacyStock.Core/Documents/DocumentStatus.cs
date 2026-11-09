namespace PharmacyStock.Core.Documents;

/// <summary>Lists the supported prescription states.</summary>
public enum DocumentStatus
{
    /// <summary>The New state.</summary>
    New = 0,

    /// <summary>The Paid state.</summary>
    Paid = 1,

    /// <summary>The Dispensed state.</summary>
    Dispensed = 2,

    /// <summary>The Cancelled state.</summary>
    Cancelled = 3,
}
