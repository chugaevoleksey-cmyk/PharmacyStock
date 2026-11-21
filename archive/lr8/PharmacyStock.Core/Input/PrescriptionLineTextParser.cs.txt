using System.Globalization;
using PharmacyStock.Core.Domain;
using PharmacyStock.Core.Errors;

namespace PharmacyStock.Core.Input;

/// <summary>Represents the PrescriptionLineTextParser type.</summary>
public static class PrescriptionLineTextParser
{
    /// <summary>The maximum number of units accepted at the input boundary.</summary>
    public const int MaximumQuantity = 1000;

    /// <summary>The maximum length of a medicine code.</summary>
    public const int MaximumCodeLength = 20;

    /// <summary>Parses one semicolon-delimited external line without input exceptions.</summary>
    /// <param name="raw">Code, quantity and invariant price separated by semicolons.</param>
    /// <param name="line">A validated line on success, otherwise null.</param>
    /// <returns>Whether all fields and their product are valid.</returns>
    public static bool TryParse(string? raw, out PrescriptionLine? line)
    {
        line = null;
        if (string.IsNullOrWhiteSpace(raw))
        {
            return false;
        }

        string[] fields = raw.Split(';');
        if (fields.Length != 3)
        {
            return false;
        }

        Result<PrescriptionLine> parsed = Parse(fields[0], fields[1], fields[2]);
        if (!parsed.IsSuccess)
        {
            return false;
        }

        line = parsed.Value;
        return true;
    }

    /// <summary>Performs the Parse operation.</summary>
    /// <param name="code">The code argument.</param>
    /// <param name="quantity">The quantity argument.</param>
    /// <param name="price">The price argument.</param>
    /// <returns>The result of the operation.</returns>
    public static Result<PrescriptionLine> Parse(string? code, string? quantity, string? price)
    {
        if (string.IsNullOrWhiteSpace(code) || code.Trim().Length > MaximumCodeLength
            || code.Contains(';', StringComparison.Ordinal))
        {
            return Result<PrescriptionLine>.Fail("Код препарату порожній, задовгий або містить роздільник");
        }

        if (!int.TryParse(quantity, NumberStyles.Integer, CultureInfo.InvariantCulture, out int count)
            || count is < 1 or > MaximumQuantity)
        {
            return Result<PrescriptionLine>.Fail("Кількість — ціле число від 1 до 1000");
        }

        if (!decimal.TryParse(
            price?.Trim(),
            NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture,
            out decimal cost)
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
