using System.Globalization;
using System.Text;
using PharmacyStock.Core.Documents;

namespace PharmacyStock.Core.Reporting;

/// <summary>Builds a culture-independent report.</summary>
public static class DocumentReport
{
    /// <summary>Builds a report without writing to any output.</summary>
    /// <param name="request">Calculation request.</param>
    /// <param name="result">Calculation result.</param>
    /// <returns>Report text using invariant formatting and LF line endings.</returns>
    public static string TextOf(TotalRequest request, TotalResult result)
    {
        DocumentGuards.EnsureRequestValid(request);
        ArgumentNullException.ThrowIfNull(result);
        DocumentGuards.EnsureAmountValid(result.Total, nameof(result.Total));
        DocumentGuards.EnsureStatusValid(result.Status);
        StringBuilder text = new();
        text.Append(FormattableString.Invariant($"Рецепт № {request.DocumentId}\n"));
        text.Append(CultureInfo.InvariantCulture, $"Пацієнт: {request.Patient.Name}\n");
        text.Append(CultureInfo.InvariantCulture, $"Пошта: {request.Patient.Email}\n");
        AppendLines(text, request.Lines);
        text.Append("Разом: ").Append(result.Total.ToString("F2", CultureInfo.InvariantCulture));
        text.Append(CultureInfo.InvariantCulture, $"\nСтан: {result.Status}\n");
        return text.ToString();
    }

    private static void AppendLines(StringBuilder text, IReadOnlyList<PrescriptionLine> lines)
    {
        foreach (PrescriptionLine line in lines)
        {
            text.Append(FormattableString.Invariant($"{line.MedicineCode} x {line.Quantity}\n"));
        }
    }
}
