using System.Globalization;
using PharmacyStock.Core;
using PharmacyStock.Core.Input;
using Xunit;

namespace PharmacyStock.Tests;

public sealed class BuildAndInputTests
{
    private static readonly string[] _reportRows = { "A1: 2", "B2: 1" };

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("SKU-1;2")]
    [InlineData("SKU-1;2;10;extra")]
    [InlineData("SKU-1;0;10.50")]
    [InlineData("SKU-1;-2;10.50")]
    [InlineData("SKU-1;1001;10.50")]
    [InlineData("SKU-1;2147483648;10.50")]
    [InlineData("SKU-1;2;-10.50")]
    [InlineData("SKU-1;2;10,50")]
    [InlineData("SKU-1;2;1,000")]
    [InlineData(";2;10")]
    [InlineData("123456789012345678901;2;10")]
    [InlineData("SKU-1;2;79228162514264337593543950335")]
    public void TryParse_InvalidInput_ReturnsFalse(string? raw)
    {
        bool ok = PrescriptionLineTextParser.TryParse(raw, out var line);

        Assert.False(ok);
        Assert.Null(line);
    }

    [Theory]
    [InlineData(" SKU-1 ;2;10.50", "SKU-1", 2, 21)]
    [InlineData("SKU-1;1;0", "SKU-1", 1, 0)]
    [InlineData("12345678901234567890;1000;1", "12345678901234567890", 1000, 1000)]
    public void TryParse_ValidBoundary_ReturnsLine(string raw, string code, int quantity, int amount)
    {
        bool ok = PrescriptionLineTextParser.TryParse(raw, out var line);

        Assert.True(ok);
        Assert.NotNull(line);
        Assert.Equal(code, line.MedicineCode);
        Assert.Equal(quantity, line.Quantity);
        Assert.Equal(amount, line.Amount);
    }

    [Theory]
    [InlineData("uk-UA")]
    [InlineData("en-US")]
    public void TryParse_DifferentCulture_UsesDecimalPoint(string culture)
    {
        CultureInfo previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
            Assert.True(PrescriptionLineTextParser.TryParse("SKU-1;2;10.50", out var line));
            Assert.Equal(21m, line!.Amount);
            Assert.False(PrescriptionLineTextParser.TryParse("SKU-1;2;10,50", out _));
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void Current_AssemblyMetadata_ReturnsSemanticVersion()
    {
        Assert.Equal("0.8.0", VersionInfo.Current());
    }

    [Fact]
    public void Build_ReportRows_UsesInvariantTitleAndPreservesRows()
    {
        ReportBuilder builder = new() { Title = "stock" };
        string result = builder.Build(_reportRows);

        Assert.Equal(string.Join(Environment.NewLine, "STOCK", "A1: 2", "B2: 1", string.Empty), result);
        Assert.Throws<ArgumentNullException>(() => builder.Build(null!));
    }
}
