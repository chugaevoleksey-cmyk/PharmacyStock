using PharmacyStock.Core.Abstractions;
using PharmacyStock.Core.Domain;

namespace PharmacyStock.Core.Services;

/// <summary>Represents the PrescriptionService type.</summary>
public sealed class PrescriptionService
{
    private readonly IPrescriptionRepository _repository;
    private readonly IPricingPolicy _pricing;

    /// <summary>Initializes a new instance of the <see cref="PrescriptionService"/> class.</summary>
    /// <param name="repository">The repository argument.</param>
    /// <param name="pricing">The pricing argument.</param>
    public PrescriptionService(IPrescriptionRepository repository, IPricingPolicy pricing)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(pricing);
        _repository = repository;
        _pricing = pricing;
    }

    /// <summary>Performs the Place operation.</summary>
    /// <param name="prescription">The prescription argument.</param>
    public void Place(Prescription prescription)
    {
        ArgumentNullException.ThrowIfNull(prescription);
        _repository.Add(prescription);
    }

    /// <summary>Performs the TotalOf operation.</summary>
    /// <param name="prescription">The prescription argument.</param>
    /// <returns>The result of the operation.</returns>
    public decimal TotalOf(Prescription prescription)
    {
        ArgumentNullException.ThrowIfNull(prescription);
        decimal total = 0m;
        foreach (PrescriptionLine line in prescription.Lines)
        {
            total += _pricing.PriceOf(line);
        }

        return total;
    }
}
