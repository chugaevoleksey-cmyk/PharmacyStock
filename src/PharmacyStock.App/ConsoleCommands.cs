using System.Globalization;
using PharmacyStock.Core.Application;
using PharmacyStock.Core.Domain;
namespace PharmacyStock.App;

internal sealed partial class ConsoleSession
{
    private void Receive(string[] parts, TextWriter output)
    {
        Count(parts, 5);
        DateOnly expires = DateOnly.ParseExact(parts[4], "yyyy-MM-dd", CultureInfo.InvariantCulture);
        Show(_service.Receive(new ReceiptInput(Number(parts[1]), parts[2], Number(parts[3]), expires)), output);
    }
    private void Create(string[] parts, TextWriter output)
    {
        Count(parts, 5);
        Show(_service.Create(Number(parts[1]), Number(parts[2]), new DispenseInput(parts[3], Number(parts[4]), null)), output);
    }
    private void Transition(string[] parts, TextWriter output)
    {
        Count(parts, 3);
        _service.Transition(Number(parts[1]), parts[2]);
        output.WriteLine("Стан: " + _repository.Prescriptions.Single(x => x.Id == Number(parts[1])).Status);
    }
    private void Dispense(string[] parts, TextWriter output)
    {
        if (parts.Length is not 3 and not 4)
        {
            throw new FormatException("dispense <код> <кількість> [рецепт]");
        }
        int quantity = Number(parts[2]);
        Validation validation = InputValidation.Line(parts[1], quantity);
        if (!validation.IsValid)
        {
            Show(validation, output);
            return;
        }
        int? id = parts.Length == 4 ? Number(parts[3]) : null;
        decimal amount = _service.Dispense(new DispenseInput(parts[1], quantity, id));
        output.WriteLine("До сплати: " + amount.ToString("F2", CultureInfo.InvariantCulture));
    }
    private void Search(string[] parts, TextWriter output)
    {
        Count(parts, 3);
        IReadOnlyList<Medicine> rows = _service.Search(parts[1], parts[2]);
        foreach (Medicine row in rows)
        {
            output.WriteLine(FormattableString.Invariant($"{row.Code}|{row.Name}|{row.Price:F2}"));
        }
        if (rows.Count == 0)
        {
            output.WriteLine("Нічого не знайдено.");
        }
    }
    private void Expiring(string[] parts, TextWriter output)
    {
        Count(parts, 2);
        IReadOnlyList<StockBatch> rows = _service.Expiring(Number(parts[1]));
        output.WriteLine("код|партія|термін|залишок");
        foreach (StockBatch row in rows)
        {
            output.WriteLine(FormattableString.Invariant($"{row.MedicineCode}|{row.Id}|{row.ExpiresOn:yyyy-MM-dd}|{row.Quantity}"));
        }
        if (rows.Count == 0)
        {
            output.WriteLine("Нічого не знайдено.");
        }
    }
    private void Save(string[] parts, TextWriter output)
    {
        Count(parts, 1);
        _repository.Save();
        _log.Write("Information", "UC-06", "Збережено");
        output.WriteLine("Дані збережено.");
    }
    private void Load(string[] parts, TextWriter output)
    {
        Count(parts, 1);
        _repository.Load();
        _log.Write("Information", "UC-06", "Завантажено");
        output.WriteLine("Дані завантажено.");
    }
}
