namespace PharmacyStock.Core.Domain;

/// <summary>
/// Рецепт пацієнта з переліком лікарських засобів.
/// </summary>
public sealed class Prescription
{
    private readonly List<PrescriptionLine> _lines = new();

    /// <summary>Gets the document or patient identifier.</summary>
    public int Id { get; init; }

    /// <summary>Gets the patient identifier.</summary>
    public int PatientId { get; init; }

    /// <summary>Gets the current document status.</summary>
    public PrescriptionStatus Status { get; private set; } =
        PrescriptionStatus.New;

    /// <summary>Gets the document creation time.</summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>Gets the prescription lines.</summary>
    public IReadOnlyList<PrescriptionLine> Lines => _lines;

    /// <summary>
    /// Додає лікарський засіб до рецепта.
    /// </summary>
    /// <param name="line">
    /// Позиція рецепта, яку потрібно додати.
    /// </param>
    public void AddLine(PrescriptionLine line)
    {
        ArgumentNullException.ThrowIfNull(line);

        _lines.Add(line);
    }

    /// <summary>
    /// Обчислює загальну вартість ліків у рецепті.
    /// </summary>
    /// <returns>
    /// Загальна вартість усіх позицій рецепта у гривнях.
    /// </returns>
    public decimal Total()
    {
        decimal total = 0m;

        foreach (PrescriptionLine line in _lines)
        {
            total += line.Amount;
        }

        return total;
    }

    /// <summary>
    /// Змінює стан рецепта за дозволеними переходами.
    /// </summary>
    /// <param name="newStatus">
    /// Новий стан рецепта.
    /// </param>
    public void ChangeStatus(PrescriptionStatus newStatus)
    {
        bool isAllowed = Status switch
        {
            PrescriptionStatus.New => newStatus is PrescriptionStatus.Approved
                or PrescriptionStatus.Cancelled or PrescriptionStatus.Expired,
            PrescriptionStatus.Approved => newStatus is PrescriptionStatus.Dispensed
                or PrescriptionStatus.Cancelled or PrescriptionStatus.Expired,
            _ => false,
        };

        if (!isAllowed)
        {
            throw new InvalidOperationException(
                "Неприпустимий перехід між станами рецепта.");
        }

        Status = newStatus;
    }
}
