using PharmacyStock.Core.Abstractions;
using PharmacyStock.Core.Domain;
using PharmacyStock.Core.Pricing;

namespace PharmacyStock.Core.Services;

/// <summary>Represents the PrescriptionService type.</summary>
public sealed class PrescriptionService
{
    private readonly IPrescriptionRepository _repository;
    private readonly IPricingPolicy _pricing;
    private readonly INotifier _notifier;

    /// <summary>Initializes a new instance of the <see cref="PrescriptionService"/> class.</summary>
    /// <param name="repository">The repository argument.</param>
    /// <param name="pricing">The pricing argument.</param>
    /// <param name="notifier">The notifier argument.</param>
    public PrescriptionService(
        IPrescriptionRepository repository, IPricingPolicy pricing, INotifier notifier)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(pricing);
        ArgumentNullException.ThrowIfNull(notifier);
        _repository = repository;
        _pricing = pricing;
        _notifier = notifier;
    }

    /// <summary>Performs the Place operation.</summary>
    /// <param name="prescription">The prescription argument.</param>
    public void Place(Prescription prescription)
    {
        ArgumentNullException.ThrowIfNull(prescription);
        decimal total = TotalOf(prescription);
        _repository.Add(prescription);
        _notifier.Notify(prescription.Id, total);
    }

    /// <summary>Performs the TotalOf operation.</summary>
    /// <param name="prescription">The prescription argument.</param>
    /// <returns>The result of the operation.</returns>
    public decimal TotalOf(Prescription prescription)
    {
        ArgumentNullException.ThrowIfNull(prescription);
        return PricingTotals.TotalOf(prescription.Lines, _pricing);
    }
}
