using System.Collections.ObjectModel;

using PharmacyStock.Core.Errors;

namespace PharmacyStock.Core.Domain;

/// <summary>Represents the Prescription type.</summary>
public sealed class Prescription
{
    private readonly List<PrescriptionLine> _lines = new();
    private readonly ReadOnlyCollection<PrescriptionLine> _readOnlyLines;

    /// <summary>Initializes a new instance of the <see cref="Prescription"/> class.</summary>
    /// <param name="id">Positive identifier.</param>
    /// <param name="patientId">Positive patient identifier.</param>
    /// <param name="createdAt">Creation date supplied by the caller.</param>
    public Prescription(int id, int patientId, DateOnly createdAt)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(patientId);
        Id = id;
        PatientId = patientId;
        CreatedAt = createdAt;
        Status = PrescriptionStatus.New;
        _readOnlyLines = _lines.AsReadOnly();
    }

    /// <summary>Gets the Id value.</summary>
    public int Id { get; }

    /// <summary>Gets the PatientId value.</summary>
    public int PatientId { get; }

    /// <summary>Gets the CreatedAt value.</summary>
    public DateOnly CreatedAt { get; }

    /// <summary>Gets the Status value.</summary>
    public PrescriptionStatus Status { get; private set; }

    /// <summary>Gets the Lines value.</summary>
    public IReadOnlyList<PrescriptionLine> Lines => _readOnlyLines;

    /// <summary>Adds an immutable line only while the prescription is new.</summary>
    /// <param name="line">Immutable medicine line.</param>
    public void AddLine(PrescriptionLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        EnsureStatus(PrescriptionStatus.New);
        _lines.Add(line);
    }

    /// <summary>Approves a nonempty new prescription.</summary>
    public void Approve()
    {
        EnsureStatus(PrescriptionStatus.New);
        if (_lines.Count == 0)
        {
            throw new DomainRuleException("Не можна підтвердити порожній рецепт.");
        }

        Status = PrescriptionStatus.Approved;
    }

    /// <summary>Marks an approved prescription as dispensed.</summary>
    public void Dispense()
    {
        EnsureStatus(PrescriptionStatus.Approved);
        Status = PrescriptionStatus.Dispensed;
    }

    /// <summary>Cancels a prescription before it is dispensed.</summary>
    public void Cancel()
    {
        EnsureActive();
        Status = PrescriptionStatus.Cancelled;
    }

    /// <summary>Marks an active prescription as expired on an external command.</summary>
    public void Expire()
    {
        EnsureActive();
        Status = PrescriptionStatus.Expired;
    }

    private void EnsureStatus(PrescriptionStatus expected)
    {
        if (Status != expected)
        {
            throw new DomainRuleException("Перехід стану заборонено.");
        }
    }

    private void EnsureActive()
    {
        if (Status is not PrescriptionStatus.New and not PrescriptionStatus.Approved)
        {
            throw new DomainRuleException("Рецепт уже закрито.");
        }
    }
}
