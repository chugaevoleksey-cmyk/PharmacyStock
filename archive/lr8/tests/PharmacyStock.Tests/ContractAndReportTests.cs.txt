using System.Globalization;
using PharmacyStock.Core.Documents;
using PharmacyStock.Core.Reporting;
using Xunit;

namespace PharmacyStock.Tests;

public sealed class ContractAndReportTests
{
    [Fact]
    public void TextOfContainsExpectedTotal()
    {
        TotalRequest request = SampleRequest();
        string text = DocumentReport.TextOf(request, DocumentTotalCalculator.Calculate(request));
        Assert.Contains("Разом: 1292.70", text);
    }

    [Fact]
    public void PrintWritesReportToOutput()
    {
        using StringWriter writer = new(CultureInfo.InvariantCulture);
        DocumentReportPrinter printer = new(writer);
        printer.Print("Разом: 10.00");
        Assert.Equal("Разом: 10.00", writer.ToString());
    }

    [Fact]
    public void CalculateRejectsNullRequest()
    {
        Assert.Throws<ArgumentNullException>(() => DocumentTotalCalculator.Calculate(null!));
    }

    [Fact]
    public void CalculateRejectsNullPatient()
    {
        Assert.Throws<ArgumentNullException>(() =>
            DocumentTotalCalculator.Calculate(SampleRequest() with { Patient = null! }));
    }

    [Fact]
    public void CalculateRejectsZeroDocumentId()
    {
        var error = Assert.Throws<ArgumentOutOfRangeException>(() =>
            DocumentTotalCalculator.Calculate(SampleRequest() with { DocumentId = 0 }));
        Assert.Equal("documentId", error.ParamName);
    }

    [Fact]
    public void CalculateRejectsWhitespaceName()
    {
        TotalRequest request = SampleRequest();
        Assert.Throws<ArgumentException>(() => DocumentTotalCalculator.Calculate(request with
        {
            Patient = request.Patient with { Name = "   " },
        }));
    }

    [Fact]
    public void CalculateRejectsInvalidEmail()
    {
        TotalRequest request = SampleRequest();
        Assert.Throws<ArgumentException>(() => DocumentTotalCalculator.Calculate(request with
        {
            Patient = request.Patient with { Email = "invalid" },
        }));
    }

    [Fact]
    public void CalculateRejectsNullLines()
    {
        Assert.Throws<ArgumentNullException>(() =>
            DocumentTotalCalculator.Calculate(SampleRequest() with { Lines = null! }));
    }

    [Fact]
    public void CalculateRejectsNullLine()
    {
        Assert.Throws<ArgumentNullException>(() =>
            DocumentTotalCalculator.Calculate(SampleRequest() with
            {
                Lines = new PrescriptionLine[] { null! },
            }));
    }

    [Fact]
    public void CalculateRejectsEmptyMedicineCode()
    {
        var line = new PrescriptionLine { MedicineCode = "", Quantity = 1, UnitPrice = 10m };
        Assert.Throws<ArgumentException>(() => DocumentGuards.EnsureLineValid(line));
    }

    [Fact]
    public void CalculateRejectsNegativeUnitPrice()
    {
        var line = new PrescriptionLine { MedicineCode = "A1", Quantity = 1, UnitPrice = -1m };
        var error = Assert.Throws<ArgumentOutOfRangeException>(() =>
            DocumentGuards.EnsureLineValid(line));
        Assert.Equal("UnitPrice", error.ParamName);
    }

    [Fact]
    public void CalculateRejectsNegativeDelivery()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            DocumentTotalCalculator.Calculate(SampleRequest() with { DeliveryPrice = -1m }));
    }

    [Fact]
    public void CalculateRejectsUnknownEnumValue()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            DocumentTotalCalculator.Calculate(SampleRequest() with { Status = (DocumentStatus)99 }));
    }

    [Fact]
    public void CancelledPrescriptionHasZeroTotal()
    {
        TotalResult result = DocumentTotalCalculator.Calculate(SampleRequest() with
        {
            Status = DocumentStatus.Cancelled,
        });
        Assert.Equal(0m, result.Total);
        Assert.Equal(DocumentStatus.Cancelled, result.Status);
    }

    [Fact]
    public void PrinterRejectsNullWriter()
    {
        Assert.Throws<ArgumentNullException>(() => new DocumentReportPrinter(null!));
    }

    [Fact]
    public void PrinterRejectsWhitespaceReport()
    {
        DocumentReportPrinter printer = new(TextWriter.Null);
        Assert.Throws<ArgumentException>(() => printer.Print("   "));
    }

    [Fact]
    public void TextOfRejectsNullResult()
    {
        Assert.Throws<ArgumentNullException>(() => DocumentReport.TextOf(SampleRequest(), null!));
    }

    [Fact]
    public void TextOfDoesNotDependOnCurrentCulture()
    {
        CultureInfo original = CultureInfo.CurrentCulture;
        try
        {
            TotalRequest request = SampleRequest();
            TotalResult result = DocumentTotalCalculator.Calculate(request);
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("uk-UA");
            string ukrainian = DocumentReport.TextOf(request, result);
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
            Assert.Equal(ukrainian, DocumentReport.TextOf(request, result));
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }

    [Theory]
    [InlineData(1000, 0)]
    [InlineData(1001, 200)]
    public void FixedCouponHasStrictThreshold(int amount, int discount)
    {
        Assert.Equal(discount, DocumentTotalCalculator.CouponDiscountOf(amount, "MINUS200"));
    }

    [Theory]
    [InlineData(999, 60)]
    [InlineData(1000, 0)]
    public void DeliveryHasExpectedThreshold(int amount, int fee)
    {
        Assert.Equal(fee, DocumentTotalCalculator.DeliveryFeeOf(amount, 60m));
    }

    [Fact]
    public void SundayDiscountIsTwoPercent()
    {
        Assert.Equal(20m, DocumentTotalCalculator.WeekendDiscountOf(1000m,
            new DateOnly(2026, 3, 8)));
    }

    [Fact]
    public void CalculatePreservesLargeSundayDiscountOrder()
    {
        TotalRequest request = SampleRequest() with
        {
            Patient = new PatientInfo { Name = "Коваль", Email = "k@ex.com" },
            Lines = new PrescriptionLine[]
            {
                new() { MedicineCode = "A1", Quantity = 1, UnitPrice = 6000m },
            },
            CouponCode = "SALE10",
            CreatedAt = new DateOnly(2026, 3, 8),
        };
        Assert.Equal(4762.80m, DocumentTotalCalculator.Calculate(request).Total);
    }

    [Fact]
    public void CalculatePreservesRegularThenVolumeThenFixedDiscount()
    {
        TotalRequest request = SampleRequest() with
        {
            Lines = new PrescriptionLine[]
            {
                new() { MedicineCode = "A1", Quantity = 1, UnitPrice = 3000m },
            },
            CouponCode = "MINUS200",
        };
        Assert.Equal(2450.50m, DocumentTotalCalculator.Calculate(request).Total);
    }

    [Fact]
    public void BulkDiscountIsAppliedBeforeDeliveryThreshold()
    {
        TotalRequest request = SampleRequest() with
        {
            Patient = new PatientInfo { Name = "Коваль", Email = "k@ex.com" },
            Lines = new PrescriptionLine[]
            {
                new() { MedicineCode = "A1", Quantity = 10, UnitPrice = 100m },
            },
            CouponCode = "MINUS200",
        };
        Assert.Equal(1010m, DocumentTotalCalculator.Calculate(request).Total);
    }

    [Fact]
    public void UnknownCouponDoesNotChangeAmount()
    {
        Assert.Equal(0m, DocumentTotalCalculator.CouponDiscountOf(3000m, "UNKNOWN"));
    }

    [Fact]
    public void PaidStateBecomesDispensed()
    {
        Assert.Equal(DocumentStatus.Dispensed,
            DocumentTotalCalculator.NextStatusOf(DocumentStatus.Paid, 100m));
    }

    [Fact]
    public void NewZeroTotalRemainsNew()
    {
        TotalRequest request = SampleRequest() with
        {
            Lines = new PrescriptionLine[]
            {
                new() { MedicineCode = "A1", Quantity = 1, UnitPrice = 0m },
            },
            DeliveryPrice = 0m,
        };
        TotalResult result = DocumentTotalCalculator.Calculate(request);
        Assert.Equal(0m, result.Total);
        Assert.Equal(DocumentStatus.New, result.Status);
    }

    [Fact]
    public void CalculateDoesNotMutateRequest()
    {
        TotalRequest request = SampleRequest();
        TotalResult first = DocumentTotalCalculator.Calculate(request);
        TotalResult second = DocumentTotalCalculator.Calculate(request);
        Assert.Equal(first, second);
        Assert.Equal(DocumentStatus.New, request.Status);
        Assert.Equal(12, request.Lines[0].Quantity);
        Assert.Equal(100m, request.Lines[0].UnitPrice);
    }

    [Fact]
    public void DecimalNegativeZeroIsAValidZeroAmount()
    {
        decimal zero = new(0, 0, 0, true, 2);
        Assert.Equal(0m, DocumentTotalCalculator.VolumeDiscountOf(zero));
        Assert.Equal(0m, DocumentTotalCalculator.ClientDiscountOf(zero, true));
    }

    private static TotalRequest SampleRequest()
    {
        return new TotalRequest
        {
            DocumentId = 1,
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
    }
}
