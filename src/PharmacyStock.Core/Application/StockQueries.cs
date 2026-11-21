using PharmacyStock.Core.Domain;
namespace PharmacyStock.Core.Application;

public sealed partial class StockService
{
    /// <summary>UC-03 створює один рядок; валідація не змінює стан при відмові.</summary>
    /// <param name="id">Новий додатний номер документа.</param>
    /// <param name="patientId">Умовний додатний номер пацієнта.</param>
    /// <param name="input">Код і кількість; PrescriptionId тут не використовується.</param>
    /// <returns>Помилка вводу або успіх.</returns>
    public Validation Create(int id, int patientId, DispenseInput input)
    {
        Validation validation = InputValidation.Line(input.Code, input.Quantity);
        if (!validation.IsValid)
        {
            return Rejected("UC-03", validation.Error!);
        }
        if (id <= 0 || patientId <= 0 || _repository.Prescriptions.Any(x => x.Id == id))
        {
            return Rejected("UC-03", "Номери рецепта та пацієнта мають бути додатними; рецепт — новим.");
        }
        Medicine? medicine = _repository.Medicines.SingleOrDefault(x => x.Code == input.Code);
        if (medicine is null)
        {
            return Rejected("UC-03", "Невідомий код препарату.");
        }
        var prescription = new Prescription(id, patientId, _clock.Today);
        prescription.AddLine(new PrescriptionLine(input.Code, input.Quantity, medicine.Price));
        Logged("UC-03", () => _repository.AddPrescription(prescription));
        return new Validation(null);
    }
    /// <summary>UC-03 виконує контрольований перехід існуючого рецепта.</summary>
    /// <param name="id">Номер документа.</param>
    /// <param name="action">approve, cancel або expire.</param>
    public void Transition(int id, string action)
    {
        Prescription prescription = _repository.Prescriptions.SingleOrDefault(x => x.Id == id)
            ?? throw new Errors.DomainRuleException("Рецепт не знайдено.");
        Action command = action switch
        {
            "approve" => prescription.Approve,
            "cancel" => prescription.Cancel,
            "expire" => prescription.Expire,
            _ => throw new ArgumentException("Невідома подія.", nameof(action)),
        };
        Logged("UC-03", command);
    }
    /// <summary>UC-04 шукає без зміни каталогу за code або name без урахування регістру.</summary>
    /// <param name="criterion">code для точного коду або name для частини назви.</param>
    /// <param name="text">Шуканий текст; порожнє name повертає весь каталог.</param>
    /// <returns>Збіги за кодом у стабільному порядку.</returns>
    public IReadOnlyList<Medicine> Search(string criterion, string text)
    {
        Func<Medicine, bool> predicate = criterion switch
        {
            "code" => x => x.Code.Equals(text, StringComparison.OrdinalIgnoreCase),
            "name" => x => x.Name.Contains(text, StringComparison.OrdinalIgnoreCase),
            _ => throw new ArgumentException("Критерій має бути code або name.", nameof(criterion)),
        };
        return _repository.Medicines.Where(predicate).OrderBy(x => x.Code, StringComparer.Ordinal).ToArray();
    }
    /// <summary>UC-05 відбирає ненульові залишки у включному інтервалі від сьогодні.</summary>
    /// <param name="days">Горизонт від 0 до MaximumHorizon включно.</param>
    /// <returns>Партії за датою, кодом і номером.</returns>
    public IReadOnlyList<StockBatch> Expiring(int days)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(days);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(days, InputValidation.MaximumHorizon);
        DateOnly limit = _clock.Today.AddDays(days);
        return _repository.Batches.Where(x => x.Quantity > 0 && x.ExpiresOn >= _clock.Today && x.ExpiresOn <= limit)
            .OrderBy(x => x.ExpiresOn).ThenBy(x => x.MedicineCode, StringComparer.Ordinal).ThenBy(x => x.Id).ToArray();
    }
}
