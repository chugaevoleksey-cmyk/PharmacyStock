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
        decimal total = DocumentTotalCalculator.Process(1, "Іваненко", "i@ex.com", true,
            lines, new DateOnly(2026, 3, 10), "", DocumentStatus.New, false, 60m, out var status);
        Assert.Equal(1292.70m, total);
        Assert.Equal(DocumentStatus.Paid, status);
    }

    [Fact]
    public void SmallPrescriptionPreserves420()
    {
        PrescriptionLine[] lines =
        {
            new() { MedicineCode = "A1", Quantity = 2, UnitPrice = 200m },
        };
        decimal total = DocumentTotalCalculator.Process(2, "Петренко", "p@ex.com", false,
            lines, new DateOnly(2026, 3, 10), "SALE10", DocumentStatus.Paid, false,
            60m, out var status);
        Assert.Equal(420m, total);
        Assert.Equal(DocumentStatus.Dispensed, status);
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
