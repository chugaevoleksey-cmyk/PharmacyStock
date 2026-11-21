using System.Globalization;

namespace PharmacyStock.App.Input;

/// <summary>Represents the QuantityInput type.</summary>
public static class QuantityInput
{
    /// <summary>Performs the CountValidQuantities operation.</summary>
    /// <param name="rows">The rows argument.</param>
    /// <returns>The result of the operation.</returns>
    public static int CountValidQuantities(IEnumerable<string?> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        int valid = 0;
        foreach (string? row in rows)
        {
            if (int.TryParse(row, NumberStyles.Integer, CultureInfo.InvariantCulture, out int quantity)
                && quantity > 0)
            {
                valid++;
            }
        }

        return valid;
    }
}
