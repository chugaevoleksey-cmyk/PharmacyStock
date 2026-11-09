using PharmacyStock.Core.Abstractions;
using PharmacyStock.Core.Domain;

namespace PharmacyStock.Core.Storage;

/// <summary>Represents the InMemoryPrescriptionRepository type.</summary>
public sealed class InMemoryPrescriptionRepository : IPrescriptionRepository
{
    private readonly Dictionary<int, Prescription> _items = new();

    /// <summary>Performs the Add operation.</summary>
    /// <param name="prescription">The prescription argument.</param>
    public void Add(Prescription prescription)
    {
        ArgumentNullException.ThrowIfNull(prescription);
        if (!_items.TryAdd(prescription.Id, prescription))
        {
            throw new InvalidOperationException("Рецепт із таким номером уже існує.");
        }
    }

    /// <summary>Performs the GetById operation.</summary>
    /// <param name="id">The id argument.</param>
    /// <returns>The result of the operation.</returns>
    public Prescription? GetById(int id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        return _items.GetValueOrDefault(id);
    }

    /// <summary>Performs the GetAll operation.</summary>
    /// <returns>The result of the operation.</returns>
    public IReadOnlyList<Prescription> GetAll()
    {
        return Array.AsReadOnly(_items.Values.ToArray());
    }
}
