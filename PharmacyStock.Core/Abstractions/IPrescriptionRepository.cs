using PharmacyStock.Core.Domain;

namespace PharmacyStock.Core.Abstractions;

/// <summary>Represents the IPrescriptionRepository type.</summary>
public interface IPrescriptionRepository
{
    /// <summary>Performs the Add operation.</summary>
    /// <param name="prescription">The prescription argument.</param>
    void Add(Prescription prescription);

    /// <summary>Performs the GetById operation.</summary>
    /// <param name="id">The id argument.</param>
    /// <returns>The result of the operation.</returns>
    Prescription? GetById(int id);

    /// <summary>Performs the GetAll operation.</summary>
    /// <returns>The result of the operation.</returns>
    IReadOnlyList<Prescription> GetAll();
}
