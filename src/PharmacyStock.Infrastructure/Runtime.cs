using System.Globalization;
using PharmacyStock.Core.Abstractions;
namespace PharmacyStock.Infrastructure;
/// <summary>Системна локальна дата робочої зміни.</summary>
public sealed class SystemClock : IClock
{
    /// <inheritdoc/>
    public DateOnly Today => DateOnly.FromDateTime(DateTime.Now);
}
/// <summary>Доповнює журнал; не приймає сирих персональних даних.</summary>
public sealed class FileAppLog : IAppLog
{
    private readonly string _path;
    /// <summary>Прив'язує журнал до локального файла.</summary>
    /// <param name="path">Шлях, заданий композиційним коренем.</param>
    public FileAppLog(string path) => _path = path;
    /// <inheritdoc/>
    public void Write(string level, string scenario, string message)
    {
        string timestamp = DateTimeOffset.Now.ToString("O", CultureInfo.InvariantCulture);
        File.AppendAllText(_path, $"{timestamp}|{level}|{scenario}|{message}{Environment.NewLine}");
    }
}
