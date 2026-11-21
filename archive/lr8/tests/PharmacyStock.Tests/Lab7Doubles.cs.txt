using PharmacyStock.Core.Abstractions;
using PharmacyStock.Core.Domain;

namespace PharmacyStock.Tests;

internal sealed class FakePrescriptionRepository : IPrescriptionRepository
{
    private readonly Dictionary<int, Prescription> _items = new();

    public int SaveCalls { get; private set; }

    public int? LastSavedId { get; private set; }

    public void Add(Prescription prescription) => _items.Add(prescription.Id, prescription);

    public Prescription? GetById(int id) => _items.GetValueOrDefault(id);

    public IReadOnlyList<Prescription> GetAll() => _items.Values.ToList().AsReadOnly();

    public void Save(Prescription prescription)
    {
        if (!_items.ContainsKey(prescription.Id))
        {
            throw new KeyNotFoundException();
        }

        _items[prescription.Id] = prescription;
        SaveCalls++;
        LastSavedId = prescription.Id;
    }
}

internal sealed class StubPricingPolicy(decimal price) : IPricingPolicy
{
    public decimal PriceOf(PrescriptionLine line) => price;
}

internal sealed class SpyNotifier : INotifier
{
    public int Calls { get; private set; }

    public void Notify(int prescriptionId, decimal total) => Calls++;
}
