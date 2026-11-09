namespace PharmacyStock.Core.Documents;

internal static class DocumentGuards
{
    public static void EnsureHeaderValid(int documentId, string patientName, string patientEmail)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(documentId);
        ArgumentException.ThrowIfNullOrWhiteSpace(patientName);
        ArgumentException.ThrowIfNullOrWhiteSpace(patientEmail);
        if (!patientEmail.Contains('@'))
        {
            throw new ArgumentException("Пошта має містити символ @.", nameof(patientEmail));
        }
    }

    public static void EnsureLineValid(PrescriptionLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        ArgumentException.ThrowIfNullOrWhiteSpace(line.MedicineCode);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(line.Quantity, nameof(line.Quantity));
        ArgumentOutOfRangeException.ThrowIfNegative(line.UnitPrice, nameof(line.UnitPrice));
    }

    public static void EnsureLinesValid(IReadOnlyList<PrescriptionLine> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);
        if (lines.Count == 0)
        {
            throw new ArgumentException("Рецепт не містить жодної позиції.", nameof(lines));
        }

        foreach (PrescriptionLine line in lines)
        {
            EnsureLineValid(line);
        }
    }

    public static void EnsureStatusValid(DocumentStatus status)
    {
        if (!Enum.IsDefined(status))
        {
            throw new ArgumentOutOfRangeException(nameof(status), status, "Невідомий стан.");
        }
    }
}
