using PharmacyStock.Core.Abstractions;
using PharmacyStock.Core.Domain;
using PharmacyStock.Core.Errors;
namespace PharmacyStock.Core.Application;
/// <summary>Виконує сценарії зміни без залежності від консолі та формату файла.</summary>
public sealed partial class StockService
{
    private readonly IStockRepository _repository;
    private readonly IClock _clock;
    private readonly IDispensingPolicy _policy;
    private readonly IAppLog _log;
    /// <summary>Отримує всі зовнішні залежності з композиційного кореня.</summary>
    /// <param name="repository">Робочий стан і збереження.</param>
    /// <param name="clock">Дата для термінів.</param>
    /// <param name="policy">Стратегія правил відпуску.</param>
    /// <param name="log">Журнал контрольованих подій.</param>
    public StockService(IStockRepository repository, IClock clock, IDispensingPolicy policy, IAppLog log)
    {
        _repository = repository;
        _clock = clock;
        _policy = policy;
        _log = log;
    }
    /// <summary>UC-02 додає нову придатну партію після перевірки всіх полів.</summary>
    /// <param name="input">Сире надходження з консолі.</param>
    /// <returns>Очікувана помилка вводу або успіх без винятку.</returns>
    public Validation Receive(ReceiptInput input)
    {
        Validation validation = InputValidation.Line(input.Code, input.Quantity);
        if (!validation.IsValid)
        {
            return validation;
        }
        if (input.BatchId <= 0 || _repository.Batches.Any(x => x.Id == input.BatchId))
        {
            return new Validation("Номер партії має бути додатним і новим.");
        }
        if (!_repository.Medicines.Any(x => x.Code == input.Code) || input.ExpiresOn < _clock.Today)
        {
            return new Validation("Невідомий препарат або прострочене надходження.");
        }
        Logged("UC-02", () => _repository.AddBatch(
            new StockBatch(input.BatchId, input.Code, input.Quantity, input.ExpiresOn)));
        return new Validation(null);
    }
    /// <summary>UC-01 перевіряє весь план перед списанням; жодної часткової зміни при відмові.</summary>
    /// <param name="input">Код, кількість та необов'язковий рецепт.</param>
    /// <returns>Сума до оплати у гривнях.</returns>
    /// <exception cref="ArgumentException">Викликач обійшов барикаду структурного вводу.</exception>
    /// <exception cref="DomainRuleException">Відсутній запас або порушено правило відпуску.</exception>
    public decimal Dispense(DispenseInput input)
    {
        Validation validation = InputValidation.Line(input.Code, input.Quantity);
        if (!validation.IsValid)
        {
            throw new ArgumentException(validation.Error, nameof(input));
        }
        Medicine medicine = RequireMedicine(input.Code);
        Prescription? prescription = _repository.Prescriptions.SingleOrDefault(x => x.Id == input.PrescriptionId);
        decimal amount = Math.Round(medicine.Price * input.Quantity, 2, MidpointRounding.AwayFromZero);
        Logged("UC-01", () =>
        {
            _policy.Check(medicine, input, prescription, _clock.Today);
            List<(StockBatch Batch, int Units)> plan = Allocate(input);
            foreach (var (batch, units) in plan)
            {
                batch.Deduct(units);
            }
            prescription?.Dispense();
        });
        return amount;
    }
    private List<(StockBatch Batch, int Units)> Allocate(DispenseInput input)
    {
        var batches = _repository.Batches.Where(x => x.MedicineCode == input.Code && x.ExpiresOn >= _clock.Today)
            .OrderBy(x => x.ExpiresOn).ThenBy(x => x.Id);
        int remaining = input.Quantity;
        var plan = new List<(StockBatch Batch, int Units)>();
        foreach (StockBatch batch in batches)
        {
            int units = Math.Min(remaining, batch.Quantity);
            if (units > 0)
            {
                plan.Add((batch, units));
                remaining -= units;
            }
        }
        if (remaining > 0)
        {
            throw new DomainRuleException("Недостатньо придатного запасу.");
        }
        return plan;
    }
    private Medicine RequireMedicine(string code) => _repository.Medicines.SingleOrDefault(x => x.Code == code)
        ?? throw new DomainRuleException("Невідомий код препарату.");
    private void Logged(string scenario, Action action)
    {
        _log.Write("Information", scenario, "Початок");
        try
        {
            action();
            _log.Write("Information", scenario, "Завершено");
        }
        catch (DomainRuleException)
        {
            _log.Write("Warning", scenario, "Операцію відхилено правилом домену");
            throw;
        }
    }
}
