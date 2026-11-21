using PharmacyStock.Core.Application;
using PharmacyStock.Core.Domain;
using PharmacyStock.Core.Errors;
using Xunit;
namespace PharmacyStock.Tests;

public sealed class DispensingTests
{
    [Theory]
    [InlineData(0, 1, 12.35)]
    [InlineData(30, 1, 12.35)]
    [InlineData(0, 2, 24.69)]
    [InlineData(30, 2, 24.69)]
    [InlineData(0, 10, 123.45)]
    [InlineData(30, 10, 123.45)]
    public void Dispense_ValidBoundary_ChargesAndCloses(int age, int units, decimal expected)
    {
        var scenario = new Scenario();
        Prescription prescription = scenario.Prescription(age);
        scenario.Batch();

        decimal actual = scenario.Service.Dispense(new DispenseInput("RX", units, 1));

        Assert.Equal(expected, actual);
        Assert.Equal(20 - units, scenario.Repository.Batches[0].Quantity);
        Assert.Equal(PrescriptionStatus.Dispensed, prescription.Status);
    }
    [Theory]
    [InlineData(31, 1)]
    [InlineData(-1, 1)]
    [InlineData(0, 11)]
    public void Dispense_InvalidRule_PreservesAllState(int age, int units)
    {
        var scenario = new Scenario();
        Prescription prescription = scenario.Prescription(age);
        scenario.Batch();

        Assert.Throws<DomainRuleException>(() => scenario.Service.Dispense(new DispenseInput("RX", units, 1)));

        Assert.Equal(20, scenario.Repository.Batches[0].Quantity);
        Assert.Equal(PrescriptionStatus.Approved, prescription.Status);
        Assert.Contains(scenario.Log.Events, x => x.StartsWith("Warning|UC-01", StringComparison.Ordinal));
    }
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(1001)]
    public void Dispense_InvalidQuantity_RejectsAtBoundary(int quantity)
    {
        var scenario = new Scenario();
        Assert.Throws<ArgumentException>(() => scenario.Service.Dispense(new DispenseInput("OTC", quantity, null)));
    }
    [Fact]
    public void Dispense_NoPrescription_RejectsRx()
    {
        var scenario = new Scenario();
        scenario.Batch();
        Assert.Throws<DomainRuleException>(() => scenario.Service.Dispense(new DispenseInput("RX", 1, null)));
    }
    [Fact]
    public void Dispense_OverCounter_RoundsAwayFromZero()
    {
        var scenario = new Scenario();
        scenario.Batch(code: "OTC");
        Assert.Equal(5.01m, scenario.Service.Dispense(new DispenseInput("OTC", 1, null)));
    }
    [Fact]
    public void Dispense_ExpiryAndFefo_UsesEarliestValidBatch()
    {
        var scenario = new Scenario();
        scenario.Prescription();
        scenario.Batch(9, -1);
        scenario.Batch(1, 0);
        scenario.Batch(4, 2);
        scenario.Service.Dispense(new DispenseInput("RX", 3, 1));
        int[] expected = { 9, 0, 2 };
        Assert.Equal(expected, scenario.Repository.Batches.Select(x => x.Quantity));
    }
    [Fact]
    public void Dispense_InsufficientStock_DoesNotPartiallyDeduct()
    {
        var scenario = new Scenario();
        scenario.Prescription();
        scenario.Batch(1);
        scenario.Batch(1);
        Assert.Throws<DomainRuleException>(() => scenario.Service.Dispense(new DispenseInput("RX", 3, 1)));
        Assert.All(scenario.Repository.Batches, x => Assert.Equal(1, x.Quantity));
    }
    [Fact]
    public void Dispense_WrongLine_Rejects()
    {
        var scenario = new Scenario();
        scenario.Prescription(code: "OTC");
        scenario.Batch();
        Assert.Throws<DomainRuleException>(() => scenario.Service.Dispense(new DispenseInput("RX", 1, 1)));
    }
    [Fact]
    public void Dispense_AbovePrescribedQuantity_Rejects()
    {
        var scenario = new Scenario();
        scenario.Prescription(units: 1);
        scenario.Batch();
        Assert.Throws<DomainRuleException>(() => scenario.Service.Dispense(new DispenseInput("RX", 2, 1)));
    }
}
