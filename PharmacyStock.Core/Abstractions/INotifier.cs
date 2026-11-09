namespace PharmacyStock.Core.Abstractions;

/// <summary>Represents the INotifier type.</summary>
public interface INotifier
{
    /// <summary>Performs the Notify operation.</summary>
    /// <param name="prescriptionId">The prescriptionId argument.</param>
    /// <param name="total">The total argument.</param>
    void Notify(int prescriptionId, decimal total);
}
