using System.Globalization;
using PharmacyStock.Core.Domain;
using PharmacyStock.Core.Errors;

namespace PharmacyStock.App.Input;

/// <summary>Represents the PrescriptionLineParser type.</summary>
public static class PrescriptionLineParser
{
    /// <summary>Performs the Parse operation.</summary>
    /// <param name="code">The code argument.</param>
    /// <param name="quantity">The quantity argument.</param>
    /// <param name="price">The price argument.</param>
    /// <returns>The result of the operation.</returns>
    public static Result<PrescriptionLine> Parse(string? code, string? quantity, string? price)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return Result<PrescriptionLine>.Fail("Код препарату порожній");
        }

        if (!int.TryParse(quantity, NumberStyles.Integer, CultureInfo.InvariantCulture, out int count)
            || count <= 0)
        {
            return Result<PrescriptionLine>.Fail("Кількість — ціле число більше 0");
        }

        const NumberStyles style = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;
        if (!decimal.TryParse(price?.Trim(), style, CultureInfo.InvariantCulture, out decimal cost)
            || cost < 0m)
        {
            return Result<PrescriptionLine>.Fail("Ціна — невід’ємне число з крапкою");
        }

        if (cost > decimal.MaxValue / count)
        {
            return Result<PrescriptionLine>.Fail("Сума позиції завелика");
        }

        return Result<PrescriptionLine>.Ok(new PrescriptionLine(code.Trim(), count, cost));
    }
}
