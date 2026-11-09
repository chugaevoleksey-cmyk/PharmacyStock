using System.Globalization;
using PharmacyStock.Core.Abstractions;
using PharmacyStock.Core.Domain;
using PharmacyStock.Core.Pricing;
using PharmacyStock.Core.Services;
using PharmacyStock.Core.Storage;

IPrescriptionRepository repository = new InMemoryPrescriptionRepository();
IPricingPolicy pricing = new DiscountPricingPolicy(0.05m);
PrescriptionService service = new(repository, pricing);
Prescription prescription = new(1, 19, new DateOnly(2026, 3, 10));
prescription.AddLine(new PrescriptionLine("MED-001", 2, 250m));
service.Place(prescription);
Console.WriteLine("Сума: " + service.TotalOf(prescription)
    .ToString("F2", CultureInfo.InvariantCulture));
