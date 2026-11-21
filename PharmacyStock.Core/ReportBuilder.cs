using System.Text;

namespace PharmacyStock.Core;

/// <summary>Builds a simple report from supplied rows.</summary>
public sealed class ReportBuilder
{
    /// <summary>Gets the report title.</summary>
    public string Title { get; init; } = "Звіт";

    /// <summary>Demonstrates a deliberately rejected compiler warning.</summary>
    /// <param name="count">Number of items.</param>
    /// <returns>The unchanged count as invariant text.</returns>
    public static string Describe(int count)
    {
        string note;
        return count.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    /// <summary>Builds the report with invariant title casing.</summary>
    /// <param name="rows">Rows to append without changing their text.</param>
    /// <returns>The complete text.</returns>
    public string Build(IReadOnlyList<string> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        var text = new StringBuilder();
        text.AppendLine(Title.ToUpperInvariant());
        foreach (var row in rows)
        {
            text.AppendLine(row);
        }

        return text.ToString();
    }
}
