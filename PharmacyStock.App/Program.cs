using System.Globalization;
using PharmacyStock.Core.Documents;

TotalRequest request = new()
{
    DocumentId = 19,
    Patient = new PatientInfo { Name = "Іваненко", Email = "i@ex.com", IsRegular = true },
    Lines = new PrescriptionLine[]
    {
        new() { MedicineCode = "A1", Quantity = 12, UnitPrice = 100m },
        new() { MedicineCode = "B2", Quantity = 1, UnitPrice = 250m },
    },
    CreatedAt = new DateOnly(2026, 3, 10),
    Status = DocumentStatus.New,
    DeliveryPrice = 60m,
};
TotalResult result = DocumentTotalCalculator.Calculate(request);
Console.WriteLine(result.Total.ToString("F2", CultureInfo.InvariantCulture));
