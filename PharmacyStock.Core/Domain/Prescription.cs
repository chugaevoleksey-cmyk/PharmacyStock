namespace PharmacyStock.Core.Domain;

/// <summary>
/// Рецепт пацієнта з переліком лікарських засобів.
/// </summary>
public sealed class Prescription
{
    private readonly List<PrescriptionLine> _lines = new();

    public int Id { get; init; }

    public int PatientId { get; init; }

    public PrescriptionStatus Status { get; private set; } =
        PrescriptionStatus.New;

    public DateTimeOffset CreatedAt { get; init; }

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
        bool isAllowed = (Status, newStatus) switch
        {
            (PrescriptionStatus.New, PrescriptionStatus.Approved) => true,
            (PrescriptionStatus.New, PrescriptionStatus.Cancelled) => true,
            (PrescriptionStatus.New, PrescriptionStatus.Expired) => true,
            (PrescriptionStatus.Approved, PrescriptionStatus.Dispensed) => true,
            (PrescriptionStatus.Approved, PrescriptionStatus.Cancelled) => true,
            (PrescriptionStatus.Approved, PrescriptionStatus.Expired) => true,
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
