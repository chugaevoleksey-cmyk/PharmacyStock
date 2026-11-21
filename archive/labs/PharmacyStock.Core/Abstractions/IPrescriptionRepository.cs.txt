using PharmacyStock.Core.Domain;

namespace PharmacyStock.Core.Abstractions;

/// <summary>Represents the IPrescriptionRepository type.</summary>
public interface IPrescriptionRepository
{
    /// <summary>Stores a prescription and rejects a duplicate identifier.</summary>
    /// <param name="prescription">Prescription aggregate.</param>
    void Add(Prescription prescription);

    /// <summary>Finds a stored prescription by its identifier.</summary>
    /// <param name="id">Positive identifier.</param>
    /// <returns>The result of the operation.</returns>
    Prescription? GetById(int id);

    /// <summary>Returns a read-only snapshot of the stored references.</summary>
    /// <returns>The result of the operation.</returns>
    IReadOnlyList<Prescription> GetAll();

    /// <summary>Saves an existing prescription; it never creates a missing record.</summary>
    /// <param name="prescription">Prescription to save.</param>
    void Save(Prescription prescription);
}
