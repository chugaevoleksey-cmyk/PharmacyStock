using PharmacyStock.Core.Documents;
using Xunit;

namespace PharmacyStock.Tests;

public sealed class CalculatorTests
{
    [Fact]
    public void RegularPatientPreserves1292()
    {
        PrescriptionLine[] lines =
        {
            new() { MedicineCode = "A1", Quantity = 12, UnitPrice = 100m },
            new() { MedicineCode = "B2", Quantity = 1, UnitPrice = 250m },
        };
        TotalResult result = DocumentTotalCalculator.Calculate(new TotalRequest
        {
            DocumentId = 1,
            Patient = new PatientInfo { Name = "Іваненко", Email = "i@ex.com", IsRegular = true },
            Lines = lines,
            CreatedAt = new DateOnly(2026, 3, 10),
            Status = DocumentStatus.New,
            DeliveryPrice = 60m,
        });
        Assert.Equal(1292.70m, result.Total);
        Assert.Equal(DocumentStatus.Paid, result.Status);
    }

    [Fact]
    public void SmallPrescriptionPreserves420()
    {
        PrescriptionLine[] lines =
        {
            new() { MedicineCode = "A1", Quantity = 2, UnitPrice = 200m },
        };
        TotalResult result = DocumentTotalCalculator.Calculate(new TotalRequest
        {
            DocumentId = 2,
            Patient = new PatientInfo { Name = "Петренко", Email = "p@ex.com" },
            Lines = lines,
            CreatedAt = new DateOnly(2026, 3, 10),
            CouponCode = "SALE10",
            Status = DocumentStatus.Paid,
            DeliveryPrice = 60m,
        });
        Assert.Equal(420m, result.Total);
        Assert.Equal(DocumentStatus.Dispensed, result.Status);
    }

    [Fact]
    public void NoLinesThrowsArgument()
    {
        Assert.Throws<ArgumentException>(() => DocumentGuards.EnsureLinesValid(
            Array.Empty<PrescriptionLine>()));
    }

    [Fact]
    public void ZeroQuantityThrowsRange()
    {
        PrescriptionLine line = new() { MedicineCode = "A1", Quantity = 0, UnitPrice = 10m };
        Assert.Throws<ArgumentOutOfRangeException>(() => DocumentGuards.EnsureLineValid(line));
    }
}
