using Microsoft.Extensions.Logging;
using PharmacyStock.Core.Errors;

namespace PharmacyStock.Core.Logging;

/// <summary>Represents the AppEvents type.</summary>
public static class AppEvents
{
    private static readonly Func<ILogger, int, IDisposable?> ScopeTemplate =
        LoggerMessage.DefineScope<int>("Prescription:{PrescriptionId}");

    private static readonly Action<ILogger, string, decimal, Exception?> StartTemplate =
        LoggerMessage.Define<string, decimal>(
            LogLevel.Debug,
            new EventId(1001, "Start"),
            "Початок {Operation}, сума {Amount}");

    private static readonly Action<ILogger, string, decimal, Exception?> CompleteTemplate =
        LoggerMessage.Define<string, decimal>(
            LogLevel.Information,
            new EventId(1002, "Complete"),
            "Завершено {Operation}, сума {Amount}");

    private static readonly Action<ILogger, int, Exception?> LineTemplate =
        LoggerMessage.Define<int>(
            LogLevel.Information,
            new EventId(1003, "LineAdded"),
            "Додано позицію, кількість {Quantity}");

    private static readonly Action<ILogger, decimal, Exception?> OverpayTemplate =
        LoggerMessage.Define<decimal>(
            LogLevel.Warning,
            new EventId(1004, "Overpayment"),
            "Переплата {Extra}");

    private static readonly Action<ILogger, string, Exception?> ErrorTemplate =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(1005, "InfrastructureFailure"),
            "Не завершено операцію {Operation}");

    private static readonly Action<ILogger, string, string, int?, Exception?> RuleTemplate =
        LoggerMessage.Define<string, string, int?>(
            LogLevel.Warning,
            new EventId(1006, "DomainRule"),
            "Правило {Rule}: {Message}; рецепт {PrescriptionId}");

    private static readonly Action<ILogger, Exception?> CriticalTemplate =
        LoggerMessage.Define(
            LogLevel.Critical,
            new EventId(1007, "Unhandled"),
            "Непередбачений збій");

    /// <summary>Performs the BeginPrescription operation.</summary>
    /// <param name="log">The log argument.</param>
    /// <param name="prescriptionId">The prescriptionId argument.</param>
    /// <returns>The result of the operation.</returns>
    public static IDisposable? BeginPrescription(ILogger log, int prescriptionId)
    {
        ArgumentNullException.ThrowIfNull(log);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(prescriptionId);
        return ScopeTemplate(log, prescriptionId);
    }

    /// <summary>Performs the Started operation.</summary>
    /// <param name="log">The log argument.</param>
    /// <param name="operation">The operation argument.</param>
    /// <param name="amount">The amount argument.</param>
    public static void Started(ILogger log, string operation, decimal amount)
    {
        ArgumentNullException.ThrowIfNull(log);
        ArgumentException.ThrowIfNullOrWhiteSpace(operation);
        StartTemplate(log, operation, amount, null);
    }

    /// <summary>Performs the Completed operation.</summary>
    /// <param name="log">The log argument.</param>
    /// <param name="operation">The operation argument.</param>
    /// <param name="amount">The amount argument.</param>
    public static void Completed(ILogger log, string operation, decimal amount)
    {
        ArgumentNullException.ThrowIfNull(log);
        ArgumentException.ThrowIfNullOrWhiteSpace(operation);
        CompleteTemplate(log, operation, amount, null);
    }

    /// <summary>Performs the LineAdded operation.</summary>
    /// <param name="log">The log argument.</param>
    /// <param name="quantity">The quantity argument.</param>
    public static void LineAdded(ILogger log, int quantity)
    {
        ArgumentNullException.ThrowIfNull(log);
        LineTemplate(log, quantity, null);
    }

    /// <summary>Performs the Overpayment operation.</summary>
    /// <param name="log">The log argument.</param>
    /// <param name="extra">The extra argument.</param>
    public static void Overpayment(ILogger log, decimal extra)
    {
        ArgumentNullException.ThrowIfNull(log);
        OverpayTemplate(log, extra, null);
    }

    /// <summary>Performs the Error operation.</summary>
    /// <param name="log">The log argument.</param>
    /// <param name="exception">The exception argument.</param>
    /// <param name="operation">The operation argument.</param>
    public static void Error(ILogger log, Exception exception, string operation)
    {
        ArgumentNullException.ThrowIfNull(log);
        ArgumentNullException.ThrowIfNull(exception);
        ArgumentException.ThrowIfNullOrWhiteSpace(operation);
        ErrorTemplate(log, operation, exception);
    }

    /// <summary>Performs the DomainRejected operation.</summary>
    /// <param name="log">The log argument.</param>
    /// <param name="exception">The exception argument.</param>
    public static void DomainRejected(ILogger log, DomainRuleException exception)
    {
        ArgumentNullException.ThrowIfNull(log);
        ArgumentNullException.ThrowIfNull(exception);
        RuleTemplate(log, exception.Rule, exception.Message, exception.PrescriptionId, null);
    }

    /// <summary>Performs the Critical operation.</summary>
    /// <param name="log">The log argument.</param>
    /// <param name="exception">The exception argument.</param>
    public static void Critical(ILogger log, Exception exception)
    {
        ArgumentNullException.ThrowIfNull(log);
        ArgumentNullException.ThrowIfNull(exception);
        CriticalTemplate(log, exception);
    }
}
