namespace PharmacyStock.Core.Abstractions;

/// <summary>Represents the INotifier type.</summary>
public interface INotifier
{
    /// <summary>Announces a stored prescription without prescribing a transport.</summary>
    /// <param name="prescriptionId">Positive prescription identifier.</param>
    /// <param name="total">Nonnegative quoted total.</param>
    void Notify(int prescriptionId, decimal total);
}
