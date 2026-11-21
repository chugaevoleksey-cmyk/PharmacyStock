namespace PharmacyStock.Core.Domain;

/// <summary>
/// Exposes a fixed status and line snapshot without inheriting mutation commands.
/// </summary>
public sealed class ArchivedPrescription
{
    private readonly Prescription _prescription;
    private readonly PrescriptionStatus _statusAtArchive;
    private readonly IReadOnlyList<PrescriptionLine> _linesAtArchive;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArchivedPrescription"/> class.
    /// </summary>
    /// <param name="prescription">Prescription aggregate.</param>
    /// <param name="archivedAt">Archive date not earlier than creation.</param>
    public ArchivedPrescription(Prescription prescription, DateOnly archivedAt)
    {
        ArgumentNullException.ThrowIfNull(prescription);
        ArgumentOutOfRangeException.ThrowIfLessThan(archivedAt, prescription.CreatedAt);
        _prescription = prescription;
        _statusAtArchive = prescription.Status;
        _linesAtArchive = Array.AsReadOnly(prescription.Lines.ToArray());
        ArchivedAt = archivedAt;
    }

    /// <summary>Gets the Id value.</summary>
    public int Id => _prescription.Id;

    /// <summary>Gets the PatientId value.</summary>
    public int PatientId => _prescription.PatientId;

    /// <summary>Gets the CreatedAt value.</summary>
    public DateOnly CreatedAt => _prescription.CreatedAt;

    /// <summary>Gets the ArchivedAt value.</summary>
    public DateOnly ArchivedAt { get; }

    /// <summary>Gets the Status value.</summary>
    public PrescriptionStatus Status => _statusAtArchive;

    /// <summary>Gets the Lines value.</summary>
    public IReadOnlyList<PrescriptionLine> Lines => _linesAtArchive;
}
