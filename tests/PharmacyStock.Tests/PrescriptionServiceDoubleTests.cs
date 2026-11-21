using Microsoft.Extensions.Logging.Abstractions;
using PharmacyStock.Core.Domain;
using PharmacyStock.Core.Errors;
using PharmacyStock.Core.Services;
using Xunit;

namespace PharmacyStock.Tests;

public sealed class PrescriptionServiceDoubleTests
{
    [Fact]
    public void TotalOf_TwoLines_SumsStubPrices()
    {
        // Arrange
        var repository = new FakePrescriptionRepository();
        var prescription = PrescriptionWithLines();
        var service = CreateService(repository, 25m);

        // Act
        decimal actual = service.TotalOf(prescription);

        // Assert
        Assert.Equal(50m, actual);
        Assert.Equal(0, repository.SaveCalls);
    }

    [Fact]
    public void Pay_ValidPrescription_SavesExactlyOnce()
    {
        // Arrange
        var repository = new FakePrescriptionRepository();
        repository.Add(PrescriptionWithLines());
        var service = CreateService(repository, 25m);

        // Act
        service.Pay(7, 50m);

        // Assert
        Assert.Equal(1, repository.SaveCalls);
        Assert.Equal(7, repository.LastSavedId);
        Assert.Equal(PrescriptionStatus.Approved, repository.GetById(7)!.Status);
    }

    [Fact]
    public void Load_UnknownId_ThrowsNamedDomainRule()
    {
        // Arrange
        var service = CreateService(new FakePrescriptionRepository(), 1m);

        // Act
        var exception = Assert.Throws<DomainRuleException>(() => service.Load(99));

        // Assert
        Assert.Equal("prescription.exists", exception.Rule);
        Assert.Equal(99, exception.PrescriptionId);
    }

    internal static Prescription PrescriptionWithLines()
    {
        var prescription = new Prescription(7, 19, new DateOnly(2026, 3, 1));
        prescription.AddLine(new PrescriptionLine("A-1", 2, 10m));
        prescription.AddLine(new PrescriptionLine("B-2", 1, 30m));
        return prescription;
    }

    internal static PrescriptionService CreateService(FakePrescriptionRepository repository, decimal price)
        => new(repository, new StubPricingPolicy(price), new SpyNotifier(), NullLogger<PrescriptionService>.Instance);
}
