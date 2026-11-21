using System.Globalization;
using PharmacyStock.Core.Domain;
using PharmacyStock.Core.Errors;
using PharmacyStock.Core.Notifications;
using PharmacyStock.Core.Pricing;
using PharmacyStock.Core.Reporting;
using Xunit;

namespace PharmacyStock.Tests;

public sealed class UncoveredBranchTests
{
    [Fact]
    public void Pay_EmptyPrescription_RejectsWithoutSaving()
    {
        // Arrange
        var repository = new FakePrescriptionRepository();
        repository.Add(new Prescription(7, 19, new DateOnly(2026, 3, 1)));
        var service = PrescriptionServiceDoubleTests.CreateService(repository, 1m);

        // Act
        var exception = Assert.Throws<DomainRuleException>(() => service.Pay(7, 1m));

        // Assert
        Assert.Equal("prescription.lines", exception.Rule);
        Assert.Equal(0, repository.SaveCalls);
        Assert.Equal(PrescriptionStatus.New, repository.GetById(7)!.Status);
    }

    [Fact]
    public void DecreaseStock_InsufficientQuantity_PreservesStock()
    {
        // Arrange
        var medicine = new Medicine("A-1", "Препарат", 10m, 3);

        // Act
        Assert.Throws<InvalidOperationException>(() => medicine.DecreaseStock(4));

        // Assert
        Assert.Equal(3, medicine.Stock);
    }

    [Fact]
    public void Restock_ValidQuantity_MakesStockAvailable()
    {
        // Arrange
        var medicine = new Medicine("A-1", "Препарат", 10m, 3);
        medicine.DecreaseStock(2);

        // Act
        medicine.Restock(4);

        // Assert
        Assert.Equal(5, medicine.Stock);
        Assert.Equal("A-1", medicine.Code);
        Assert.Equal("Препарат", medicine.Name);
        Assert.Equal(10m, medicine.Price);
    }

    [Fact]
    public void Patient_InvalidEmail_RejectsNamedParameter()
    {
        // Arrange
        const string invalidEmail = "no-at-sign";

        // Act
        var exception = Assert.Throws<ArgumentException>(() => new Patient(19, "Іван", invalidEmail));

        // Assert
        Assert.Equal("email", exception.ParamName);
    }

    [Fact]
    public void BuildCsv_Prescription_UsesInvariantAmountsAndStatus()
    {
        // Arrange
        var prescription = PrescriptionServiceDoubleTests.PrescriptionWithLines();
        var builder = new PrescriptionCsvReport(new RegularPricingPolicy());
        CultureInfo previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("uk-UA");

            // Act
            string actual = builder.BuildCsv(new[] { prescription });

            // Assert
            Assert.Equal("id;total;status\n7;50.00;New\n", actual);
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void Notify_ValidAmount_WritesToMemoryWithoutOwningWriter()
    {
        // Arrange
        using var writer = new StringWriter(CultureInfo.InvariantCulture);
        var notifier = new TextWriterNotifier(writer);

        // Act
        notifier.Notify(7, 50m);

        // Assert
        Assert.Equal("Збережено рецепт № 7. Сума: 50.00" + Environment.NewLine, writer.ToString());
        writer.Write("still open");
        Assert.EndsWith("still open", writer.ToString(), StringComparison.Ordinal);
    }
}
