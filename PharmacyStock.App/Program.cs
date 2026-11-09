using System.Globalization;
using PharmacyStock.Core.Abstractions;
using PharmacyStock.Core.Domain;
using PharmacyStock.Core.Notifications;
using PharmacyStock.Core.Pricing;
using PharmacyStock.Core.Reporting;
using PharmacyStock.Core.Services;
using PharmacyStock.Core.Storage;

InMemoryPrescriptionRepository repository = new();
Dictionary<string, IPricingPolicy> policies = new(StringComparer.OrdinalIgnoreCase)
{
    ["standard"] = new RegularPricingPolicy(),
    ["regular"] = new DiscountPricingPolicy(0.05m),
    ["vip"] = new VipPricingPolicy(),
    ["staff"] = new DiscountPricingPolicy(0.30m),
};
string patientType = args.Length > 0 ? args[0] : "regular";
IPricingPolicy pricing = policies.TryGetValue(patientType, out IPricingPolicy? found)
    ? found : policies["standard"];
INotifier notifier = new TextWriterNotifier(Console.Out);
PrescriptionService service = new(repository, pricing, notifier);
PrescriptionCsvReport csvReport = new(pricing);

Patient patient = new(19, "Іваненко", "i@ex.com", isRegular: true);
Medicine medicine = new("MED-001", "Навчальний препарат", 250m, 20);
Prescription prescription = new(1, patient.Id, new DateOnly(2026, 3, 10));
prescription.AddLine(new PrescriptionLine(medicine.Code, 2, medicine.Price));
service.Place(prescription);
prescription.Approve();
Console.WriteLine("Сума: " + service.TotalOf(prescription)
    .ToString("F2", CultureInfo.InvariantCulture));
Console.WriteLine($"Стан: {prescription.Status}");
prescription.Dispense();
ArchivedPrescription archived = new(prescription, new DateOnly(2026, 3, 11));
Console.WriteLine(FormattableString.Invariant($"Архів: {archived.Id}; стан: {archived.Status}"));
Console.Write(csvReport.BuildCsv(repository.GetAll()));
