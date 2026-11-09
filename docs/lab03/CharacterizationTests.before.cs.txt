using PharmacyStock.Core.Lab03Legacy;
using Xunit;

namespace PharmacyStock.Tests;

public class LegacyProcessorTests
{
    [Fact]
    public void Process_RegularPatient_Returns1292()
    {
        List<PrescriptionLine> lines = new()
        {
            new() { MedicineCode = "A1", Quantity = 12, UnitPrice = 100m },
            new() { MedicineCode = "B2", Quantity = 1, UnitPrice = 250m },
        };
        decimal total = LegacyProcessor.Process(1, "Іваненко", "i@ex.com", true, lines,
            new DateOnly(2026, 3, 10), "", "New", false, 60m, out string next);
        Assert.Equal(1292.70m, total);
        Assert.Equal("Paid", next);
    }

    [Fact]
    public void Process_SmallPrescription_AddsDelivery()
    {
        List<PrescriptionLine> lines = new()
        {
            new() { MedicineCode = "A1", Quantity = 2, UnitPrice = 200m },
        };
        decimal total = LegacyProcessor.Process(2, "Петренко", "p@ex.com", false, lines,
            new DateOnly(2026, 3, 10), "SALE10", "Paid", false, 60m, out string next);
        Assert.Equal(420m, total);
        Assert.Equal("Shipped", next);
    }

    [Fact]
    public void Process_NoLines_ReturnsMinusOne()
    {
        decimal total = LegacyProcessor.Process(3, "Коваль", "k@ex.com", false,
            new List<PrescriptionLine>(), new DateOnly(2026, 3, 10), "", "New", false,
            60m, out _);
        Assert.Equal(-1m, total);
    }
}
