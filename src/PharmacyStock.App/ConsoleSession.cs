using System.Globalization;
using System.Text.Json;
using PharmacyStock.Core.Abstractions;
using PharmacyStock.Core.Application;
using PharmacyStock.Core.Domain;
using PharmacyStock.Core.Errors;
namespace PharmacyStock.App;

internal sealed partial class ConsoleSession
{
    private readonly StockService _service;
    private readonly IStockRepository _repository;
    private readonly IAppLog _log;
    private readonly Dictionary<string, Action<string[], TextWriter>> _commands;
    public ConsoleSession(StockService service, IStockRepository repository, IAppLog log)
    {
        _service = service;
        _repository = repository;
        _log = log;
        _commands = new(StringComparer.OrdinalIgnoreCase)
        {
            ["receive"] = Receive,
            ["create"] = Create,
            ["state"] = Transition,
            ["dispense"] = Dispense,
            ["search"] = Search,
            ["expiring"] = Expiring,
            ["save"] = Save,
            ["load"] = Load,
        };
    }
    public void Run(TextReader input, TextWriter output)
    {
        output.WriteLine("PharmacyStock 1.0 — умовні дані; help — команди, exit — вихід.");
        string? line;
        while ((line = input.ReadLine()) is not null)
        {
            if (line.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            Execute(line, output);
        }
    }
    private void Execute(string line, TextWriter output)
    {
        string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
        {
            return;
        }
        try
        {
            if (_commands.TryGetValue(parts[0], out var command))
            {
                command(parts, output);
            }
            else
            {
                Help(output);
            }
        }
        catch (Exception exception) when (IsUserFacing(exception))
        {
            _log.Write(exception is DomainRuleException or ArgumentException or FormatException ? "Warning" : "Error", "Console", exception.GetType().Name);
            output.WriteLine("Операцію відхилено: " + SafeMessage(exception));
        }
    }
    private static bool IsUserFacing(Exception exception) => exception is
        DomainRuleException or FormatException or ArgumentException or IOException or UnauthorizedAccessException or JsonException;
    private static string SafeMessage(Exception exception) => exception is DomainRuleException or ArgumentException or FormatException
        ? exception.Message : "Помилка файла: перевірте шлях, доступ і структуру JSON.";
    private static void Help(TextWriter output)
    {
        output.WriteLine("receive <партія> <код> <кількість> <yyyy-MM-dd>");
        output.WriteLine("create <рецепт> <пацієнт-id> <код> <кількість>");
        output.WriteLine("state <рецепт> <approve|cancel|expire>");
        output.WriteLine("dispense <код> <кількість> [рецепт]");
        output.WriteLine("search <code|name> <текст> | expiring <днів 0..90> | save | load | exit");
    }
    private static void Count(string[] parts, int expected)
    {
        if (parts.Length != expected)
        {
            throw new FormatException("Неправильна кількість аргументів; введіть help.");
        }
    }
    private static int Number(string text) => int.Parse(text, CultureInfo.InvariantCulture);
    private static void Show(Validation result, TextWriter output) => output.WriteLine(result.Error ?? "Виконано.");
}
