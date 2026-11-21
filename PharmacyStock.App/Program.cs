using System.Globalization;
using Microsoft.Extensions.Logging;
using PharmacyStock.App.Infrastructure;
using PharmacyStock.App.Input;
using PharmacyStock.Core;
using PharmacyStock.Core.Abstractions;
using PharmacyStock.Core.Domain;
using PharmacyStock.Core.Errors;
using PharmacyStock.Core.Logging;
using PharmacyStock.Core.Notifications;
using PharmacyStock.Core.Pricing;
using PharmacyStock.Core.Services;
using PharmacyStock.Core.Storage;

namespace PharmacyStock.App;

/// <summary>Represents the Program type.</summary>
public static class Program
{
    /// <summary>Performs the Main operation.</summary>
    /// <param name="args">The args argument.</param>
    /// <returns>The result of the operation.</returns>
    public static int Main(string[] args)
    {
        Console.WriteLine($"PharmacyStock v{VersionInfo.Current()}");
        using ILoggerFactory factory = LoggerFactory.Create(builder =>
        {
            builder.AddSimpleConsole(options =>
            {
                options.TimestampFormat = "HH:mm:ss ";
                options.IncludeScopes = true;
            });
            builder.SetMinimumLevel(LogLevel.Debug);
        });
        ILogger log = factory.CreateLogger("PharmacyStock.App");
        try
        {
            RunApplication(args, factory);
            return 0;
        }
        catch (DomainRuleException exception)
        {
            AppEvents.DomainRejected(log, exception);
            Console.WriteLine("Операцію не виконано: " + exception.Message);
            return 1;
        }
        catch (IOException exception)
        {
            AppEvents.Error(log, exception, "Files");
            Console.WriteLine("Не вдалося записати файл. Перевірте доступ і повторіть.");
            return 2;
        }
        catch (UnauthorizedAccessException exception)
        {
            AppEvents.Error(log, exception, "FileAccess");
            Console.WriteLine("Немає доступу до файлу аудиту або звіту.");
            return 2;
        }
        catch (Exception exception)
        {
            AppEvents.Critical(log, exception);
            Console.WriteLine("Внутрішня помилка застосунку APP-UNEXPECTED.");
            return 3;
        }
    }

    private static void RunApplication(string[] args, ILoggerFactory factory)
    {
        InMemoryPrescriptionRepository repository = new();
        IPricingPolicy pricing = new DiscountPricingPolicy(0.05m);
        INotifier notifier = new TextWriterNotifier(Console.Out);
        PrescriptionService service = new(
            repository, pricing, notifier, factory.CreateLogger<PrescriptionService>());
        repository.Add(new Prescription(1, 19, new DateOnly(2026, 3, 10)));
        bool demoError = args.Contains("--demo-error", StringComparer.Ordinal);
        bool demonstration = demoError || args.Contains("--demo", StringComparer.Ordinal);
        using StringReader? sample = demonstration
            ? new StringReader("MED-001\nдві\n250\nMED-001\n2\n250\n") : null;
        if (demonstration)
        {
            Console.WriteLine("Демо: спочатку кількість «дві», потім 2.");
        }

        string runtime = Path.Combine(Environment.CurrentDirectory, "runtime");
        Directory.CreateDirectory(runtime);
        string auditPath = Path.Combine(runtime, "audit.log");
        using (FileAuditLog audit = new(auditPath))
        {
            audit.Write("start", 1);
            PrescriptionConsole session = new(sample ?? Console.In, Console.Out);
            if (!session.ReadAndAddLine(service, 1))
            {
                return;
            }

            decimal total = service.TotalOf(service.Load(1));
            service.Pay(1, total + 10m);
            audit.Write("pay", 1);
            Console.WriteLine("Сума: " + total.ToString("F2", CultureInfo.InvariantCulture));
            Console.WriteLine($"Стан: {service.Load(1).Status}");
            ReportExporter exporter = new(service, factory.CreateLogger<ReportExporter>());
            exporter.ExportReport(Path.Combine(runtime, "report.txt"), service.Load(1));
            audit.Write("export", 1);
            if (demoError)
            {
                DemonstrateExportFailure(runtime, exporter, service.Load(1));
            }
        }

        using FileStream reopened = new(auditPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        Console.WriteLine("Аудит закрито: файл повторно відкрито ексклюзивно.");
    }

    private static void DemonstrateExportFailure(
        string runtime, ReportExporter exporter, Prescription prescription)
    {
        string missingDirectory = Path.Combine(runtime, Guid.NewGuid().ToString("N"));
        try
        {
            exporter.ExportReport(Path.Combine(missingDirectory, "report.txt"), prescription);
        }
        catch (IOException)
        {
            Console.WriteLine("Контрольний збій експорту оброблено; програма продовжує роботу.");
        }
    }
}
