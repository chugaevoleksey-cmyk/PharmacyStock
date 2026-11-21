namespace PharmacyStock.Core.Application;
/// <summary>Надходження до перевірки на межі прикладного шару.</summary>
/// <param name="BatchId">Новий додатний номер партії.</param>
/// <param name="Code">Код препарату.</param>
/// <param name="Quantity">Число одиниць 1..1000.</param>
/// <param name="ExpiresOn">Сьогодні або пізніше.</param>
public sealed record ReceiptInput(int BatchId, string Code, int Quantity, DateOnly ExpiresOn);
/// <summary>Запит на відпуск, який не довіряє сирому вводу.</summary>
/// <param name="Code">Код препарату.</param>
/// <param name="Quantity">Додатна кількість одиниць.</param>
/// <param name="PrescriptionId">Номер рецепта; null для безрецептурного відпуску.</param>
public sealed record DispenseInput(string Code, int Quantity, int? PrescriptionId);
/// <summary>Результат перевірки очікуваних помилок вводу без винятку.</summary>
/// <param name="Error">Null означає допустимі дані; інакше повідомлення для повтору.</param>
public sealed record Validation(string? Error)
{
    /// <summary>Чи можна передати дані до правил домену.</summary>
    public bool IsValid => Error is null;
}
/// <summary>Єдина барикада структурної валідації прикладного шару.</summary>
public static class InputValidation
{
    /// <summary>Найбільше одиниць в одному навчальному запиті.</summary>
    public const int MaximumQuantity = 1000;
    /// <summary>Верхня межа навчальної ціни; запобігає переповненню при множенні.</summary>
    public const decimal MaximumPrice = 1000000m;
    /// <summary>Найбільший горизонт звіту в днях.</summary>
    public const int MaximumHorizon = 90;
    /// <summary>Перевіряє код і кількість до виконання сценарію.</summary>
    /// <param name="code">Сирий код; null чи пробіли недопустимі.</param>
    /// <param name="quantity">Сире число одиниць.</param>
    /// <returns>Повідомлення очікуваної помилки або успіх.</returns>
    public static Validation Line(string? code, int quantity)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return new Validation("Код не може бути порожнім.");
        }
        return quantity is <= 0 or > MaximumQuantity
            ? new Validation("Кількість має бути від 1 до 1000.") : new Validation(null);
    }
}
