using System.Globalization;
using PharmacyStock.Core.Domain;

Prescription prescription = new()
{
    Id = 1,
    PatientId = 19,
    CreatedAt = DateTimeOffset.Now,
};

prescription.AddLine(new PrescriptionLine
{
    MedicineCode = "MED-001",
    Quantity = 2,
    UnitPrice = 149.50m,
});

prescription.AddLine(new PrescriptionLine
{
    MedicineCode = "MED-002",
    Quantity = 1,
    UnitPrice = 89.90m,
});

string total = prescription.Total()
    .ToString("F2", CultureInfo.InvariantCulture);

Console.WriteLine($"Рецепт #{prescription.Id}");
Console.WriteLine($"Стан: {prescription.Status}");
Console.WriteLine($"Позицій: {prescription.Lines.Count}");
Console.WriteLine($"Сума: {total}");
