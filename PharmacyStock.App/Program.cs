using System.Globalization;
using PharmacyStock.App.Input;
using PharmacyStock.Core.Abstractions;
using PharmacyStock.Core.Domain;
using PharmacyStock.Core.Notifications;
using PharmacyStock.Core.Pricing;
using PharmacyStock.Core.Services;
using PharmacyStock.Core.Storage;

InMemoryPrescriptionRepository repository = new();
IPricingPolicy pricing = new DiscountPricingPolicy(0.05m);
INotifier notifier = new TextWriterNotifier(Console.Out);
PrescriptionService service = new(repository, pricing, notifier);
repository.Add(new Prescription(1, 19, new DateOnly(2026, 3, 10)));
bool demonstration = args.Contains("--demo", StringComparer.Ordinal);
using StringReader? sample = demonstration
    ? new StringReader("MED-001\nдві\n250\nMED-001\n2\n250\n") : null;
if (demonstration)
{
    Console.WriteLine("Демо: спочатку кількість «дві», потім 2.");
}

PrescriptionConsole session = new(sample ?? Console.In, Console.Out);
if (session.ReadAndAddLine(service, 1))
{
    decimal total = service.TotalOf(service.Load(1));
    service.Pay(1, total);
    Console.WriteLine("Сума: " + total.ToString("F2", CultureInfo.InvariantCulture));
    Console.WriteLine($"Стан: {service.Load(1).Status}");
}
