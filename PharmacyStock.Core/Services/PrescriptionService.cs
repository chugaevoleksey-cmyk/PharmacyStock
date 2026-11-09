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

    /// <summary>
    /// Initializes a new instance of the <see cref="PrescriptionService"/> class.
    /// </summary>
    /// <param name="repository">Storage dependency supplied by the composition root.</param>
    /// <param name="pricing">Pricing dependency supplied by the composition root.</param>
    /// <param name="notifier">Notification dependency supplied by the composition root.</param>
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

    /// <summary>Stores a prescription and then notifies through the injected adapter.</summary>
    /// <param name="prescription">Prescription aggregate.</param>
    public void Place(Prescription prescription)
    {
        ArgumentNullException.ThrowIfNull(prescription);
        decimal total = TotalOf(prescription);
        _repository.Add(prescription);
        _notifier.Notify(prescription.Id, total);
    }

    /// <summary>Quotes the total with line pricing and the shared threshold discount.</summary>
    /// <param name="prescription">Prescription aggregate.</param>
    /// <returns>The result of the operation.</returns>
    public decimal TotalOf(Prescription prescription)
    {
        ArgumentNullException.ThrowIfNull(prescription);
        return PricingTotals.TotalOf(prescription.Lines, _pricing);
    }
}
