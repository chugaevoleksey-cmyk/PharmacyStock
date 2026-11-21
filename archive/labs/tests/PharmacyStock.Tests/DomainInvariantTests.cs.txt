using Microsoft.Extensions.Logging.Abstractions;
using PharmacyStock.Core.Abstractions;
using PharmacyStock.Core.Domain;
using PharmacyStock.Core.Pricing;
using PharmacyStock.Core.Services;
using PharmacyStock.Core.Storage;
using Xunit;

namespace PharmacyStock.Tests;

public sealed class DomainInvariantTests
{
    [Fact]
    public void PrescriptionLineWithZeroQuantityThrowsRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new PrescriptionLine("MED-1", 0, 10m));
        Assert.Throws<ArgumentOutOfRangeException>(() => new PrescriptionLine("MED-1", 1, -1m));
        Assert.Throws<ArgumentException>(() => new PrescriptionLine("  ", 1, 10m));
        PrescriptionLine valid = new("MED-1", 2, 100m);
        Assert.Equal(200m, valid.Amount);
    }

    [Fact]
    public void PrescriptionAfterApproveRejectsNewLines()
    {
        Prescription prescription = NewPrescription(1);
        prescription.AddLine(new PrescriptionLine("MED-1", 1, 10m));
        ArchivedPrescription archived = new(prescription, new DateOnly(2026, 3, 11));
        ICollection<PrescriptionLine> view = (ICollection<PrescriptionLine>)prescription.Lines;
        Assert.Throws<NotSupportedException>(() =>
            view.Add(new PrescriptionLine("MED-2", 1, 5m)));
        prescription.Approve();
        Assert.Throws<InvalidOperationException>(() =>
            prescription.AddLine(new PrescriptionLine("MED-2", 1, 5m)));
        Assert.Equal(PrescriptionStatus.Approved, prescription.Status);
        prescription.Dispense();
        Assert.Throws<InvalidOperationException>(() => prescription.Approve());
        Assert.Equal(PrescriptionStatus.New, archived.Status);
        Assert.Single(archived.Lines);
    }

    [Fact]
    public void TotalOfWithTenPercentPolicyReturns180()
    {
        InMemoryPrescriptionRepository repository = new();
        RecordingNotifier notifier = new();
        PrescriptionService service = new(
            repository,
            new DiscountPricingPolicy(0.10m),
            notifier,
            NullLogger<PrescriptionService>.Instance);
        Prescription prescription = NewPrescription(2);
        prescription.AddLine(new PrescriptionLine("MED-1", 2, 100m));
        Assert.Equal(180m, service.TotalOf(prescription));
        Assert.Equal(0, notifier.Count);
        service.Place(prescription);
        Assert.Same(prescription, repository.GetById(2));
        Assert.Equal(1, notifier.Count);
        Assert.Equal(180m, notifier.LastTotal);
        Assert.Throws<InvalidOperationException>(() => service.Place(prescription));
        Assert.Equal(1, notifier.Count);
        IReadOnlyList<Prescription> snapshot = repository.GetAll();
        repository.Add(NewPrescription(3));
        Assert.Single(snapshot);
        Assert.Equal(2, repository.GetAll().Count);
        Prescription large = NewPrescription(4);
        large.AddLine(new PrescriptionLine("MED-2", 2, 1000m));
        Assert.Equal(1750m, service.TotalOf(large));
    }

    private static Prescription NewPrescription(int id)
    {
        return new Prescription(id, 19, new DateOnly(2026, 3, 10));
    }

    private sealed class RecordingNotifier : INotifier
    {
        public int Count { get; private set; }

        public decimal LastTotal { get; private set; }

        public void Notify(int prescriptionId, decimal total)
        {
            Count++;
            LastTotal = total;
        }
    }
}
