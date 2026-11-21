using PharmacyStock.Core.Domain;
using PharmacyStock.Core.Errors;
using PharmacyStock.Core.Input;

namespace PharmacyStock.App.Input;

/// <summary>Adapts console fields to the shared validated input boundary.</summary>
public static class PrescriptionLineParser
{
    /// <summary>Parses console fields using the shared validation rules.</summary>
    /// <param name="code">The medicine code.</param>
    /// <param name="quantity">The number of units.</param>
    /// <param name="price">The invariant decimal price.</param>
    /// <returns>A validated line or a user-facing validation error.</returns>
    public static Result<PrescriptionLine> Parse(string? code, string? quantity, string? price)
    {
        return PrescriptionLineTextParser.Parse(code, quantity, price);
    }
}
