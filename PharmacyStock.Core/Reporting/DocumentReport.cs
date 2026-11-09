using System.Globalization;
using System.Text;
using PharmacyStock.Core.Documents;

namespace PharmacyStock.Core.Reporting;

public static class DocumentReport
{
    public static string TextOf(TotalRequest request, TotalResult result)
    {
        DocumentGuards.EnsureRequestValid(request);
        ArgumentNullException.ThrowIfNull(result);
        ArgumentOutOfRangeException.ThrowIfNegative(result.Total);
        DocumentGuards.EnsureStatusValid(result.Status);
        StringBuilder text = new();
        text.Append(FormattableString.Invariant($"Рецепт № {request.DocumentId}\n"));
        text.Append($"Пацієнт: {request.Patient.Name}\n");
        text.Append($"Пошта: {request.Patient.Email}\n");
        AppendLines(text, request.Lines);
        text.Append("Разом: ").Append(result.Total.ToString("F2", CultureInfo.InvariantCulture));
        text.Append($"\nСтан: {result.Status}\n");
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
