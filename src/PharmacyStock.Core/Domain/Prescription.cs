using System.Collections.ObjectModel;

using PharmacyStock.Core.Errors;

namespace PharmacyStock.Core.Domain;

/// <summary>Рецепт зі станом і закритою колекцією незмінних позицій.</summary>
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

    /// <summary>Умовний номер документа, унікальний у сховищі.</summary>
    public int Id { get; }

    /// <summary>Умовний номер пацієнта без персональних даних.</summary>
    public int PatientId { get; }

    /// <summary>Початкова дата для перевірки строку рецепта.</summary>
    public DateOnly CreatedAt { get; }

    /// <summary>Поточний стан, змінюваний лише дозволеними командами.</summary>
    public PrescriptionStatus Status { get; private set; }

    /// <summary>Позиції лише для читання; додавання дозволено у стані New.</summary>
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
