using PharmacyStock.Core.Abstractions;
using PharmacyStock.Core.Domain;

namespace PharmacyStock.Core.Storage;

/// <summary>Represents the InMemoryPrescriptionRepository type.</summary>
public sealed class InMemoryPrescriptionRepository : IPrescriptionRepository
{
    private readonly Dictionary<int, Prescription> _items = new();

    /// <summary>Stores a prescription and rejects a duplicate identifier.</summary>
    /// <param name="prescription">Prescription aggregate.</param>
    public void Add(Prescription prescription)
    {
        ArgumentNullException.ThrowIfNull(prescription);
        if (!_items.TryAdd(prescription.Id, prescription))
        {
            throw new InvalidOperationException("Рецепт із таким номером уже існує.");
        }
    }

    /// <summary>Finds a stored prescription by its identifier.</summary>
    /// <param name="id">Positive identifier.</param>
    /// <returns>The matching prescription, or null when it is absent.</returns>
    public Prescription? GetById(int id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        return _items.GetValueOrDefault(id);
    }

    /// <summary>Returns a read-only snapshot of the stored references.</summary>
    /// <returns>The result of the operation.</returns>
    public IReadOnlyList<Prescription> GetAll()
    {
        return Array.AsReadOnly(_items.Values.ToArray());
    }
}
