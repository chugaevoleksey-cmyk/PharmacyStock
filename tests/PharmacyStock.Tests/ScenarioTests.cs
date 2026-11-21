using PharmacyStock.Core.Application;
using PharmacyStock.Core.Domain;
using PharmacyStock.Core.Errors;
using Xunit;
namespace PharmacyStock.Tests;

public sealed class ScenarioTests
{
    [Theory]
    [InlineData("", 1)]
    [InlineData(null, 1)]
    [InlineData("OTC", -1)]
    [InlineData("missing", 1)]
    public void Receive_InvalidInput_ReturnsMessage(string? code, int units)
    {
        var scenario = new Scenario();
        Validation result = scenario.Service.Receive(new ReceiptInput(1, code!, units, scenario.Clock.Today));
        Assert.False(result.IsValid);
        Assert.Empty(scenario.Repository.Batches);
    }
    [Fact]
    public void Receive_Today_AcceptsAndRejectsDuplicate()
    {
        var scenario = new Scenario();
        var input = new ReceiptInput(1, "OTC", 5, scenario.Clock.Today);
        Assert.True(scenario.Service.Receive(input).IsValid);
        Assert.False(scenario.Service.Receive(input).IsValid);
        Assert.Equal(5, Assert.Single(scenario.Repository.Batches).Quantity);
    }
    [Fact]
    public void Receive_Expired_DoesNotChangeStock()
    {
        var scenario = new Scenario();
        Assert.False(scenario.Service.Receive(new ReceiptInput(1, "OTC", 1, scenario.Clock.Today.AddDays(-1))).IsValid);
        Assert.Empty(scenario.Repository.Batches);
    }
    [Fact]
    public void Create_Valid_StoresNewDocument()
    {
        var scenario = new Scenario();
        Assert.True(scenario.Service.Create(19, 1, new DispenseInput("RX", 2, null)).IsValid);
        Assert.Equal(PrescriptionStatus.New, Assert.Single(scenario.Repository.Prescriptions).Status);
        Assert.False(scenario.Service.Create(19, 1, new DispenseInput("RX", 2, null)).IsValid);
    }
    [Fact]
    public void Create_UnknownCode_StoresNothing()
    {
        var scenario = new Scenario();
        Assert.False(scenario.Service.Create(1, 1, new DispenseInput("?", 1, null)).IsValid);
        Assert.Empty(scenario.Repository.Prescriptions);
    }
    [Theory]
    [InlineData("approve", PrescriptionStatus.Approved)]
    [InlineData("cancel", PrescriptionStatus.Cancelled)]
    [InlineData("expire", PrescriptionStatus.Expired)]
    public void Transition_New_FollowsTable(string action, PrescriptionStatus expected)
    {
        var scenario = new Scenario();
        scenario.Service.Create(1, 1, new DispenseInput("RX", 2, null));
        scenario.Service.Transition(1, action);
        Assert.Equal(expected, scenario.Repository.Prescriptions[0].Status);
    }
    [Theory]
    [InlineData("cancel")]
    [InlineData("expire")]
    public void Transition_Approved_AllowsClosing(string action)
    {
        var scenario = new Scenario();
        scenario.Prescription();
        scenario.Service.Transition(1, action);
        Assert.NotEqual(PrescriptionStatus.Approved, scenario.Repository.Prescriptions[0].Status);
    }
    [Theory]
    [InlineData("cancel")]
    [InlineData("expire")]
    [InlineData("approve")]
    public void Transition_Dispensed_ThrowsDomainException(string action)
    {
        var scenario = new Scenario();
        scenario.Prescription().Dispense();
        Assert.Throws<DomainRuleException>(() => scenario.Service.Transition(1, action));
    }
    [Fact]
    public void Dispense_Repeated_RejectsWithoutFurtherDeduction()
    {
        var scenario = new Scenario();
        scenario.Prescription();
        scenario.Batch();
        scenario.Service.Dispense(new DispenseInput("RX", 1, 1));
        Assert.Throws<DomainRuleException>(() => scenario.Service.Dispense(new DispenseInput("RX", 1, 1)));
        Assert.Equal(19, scenario.Repository.Batches[0].Quantity);
    }
}
