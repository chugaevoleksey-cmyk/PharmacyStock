namespace PharmacyStock.Core.Domain;

/// <summary>Незмінна позиція рецепта з ціною, зафіксованою при створенні.</summary>
public sealed record PrescriptionLine
{
    /// <summary>Initializes a new instance of the <see cref="PrescriptionLine"/> class.</summary>
    /// <param name="medicineCode">Nonblank medicine code.</param>
    /// <param name="quantity">Positive number of units.</param>
    /// <param name="unitPrice">Nonnegative price fixed for this line.</param>
    // TODO TD-03 (28.11.2026): узгодити ці захисні перевірки з перевіреним запитом.
    public PrescriptionLine(string medicineCode, int quantity, decimal unitPrice)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(medicineCode);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        ArgumentOutOfRangeException.ThrowIfLessThan(unitPrice, 0m);

        MedicineCode = medicineCode;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    /// <summary>Ключ препарату каталогу.</summary>
    public string MedicineCode { get; }

    /// <summary>Додатна кількість дозволених одиниць.</summary>
    public int Quantity { get; }

    /// <summary>Ціна одиниці на момент створення рецепта.</summary>
    public decimal UnitPrice { get; }

    /// <summary>Неокруглена сума позиції; правило відпуску округлює підсумок.</summary>
    public decimal Amount => Quantity * UnitPrice;
}
