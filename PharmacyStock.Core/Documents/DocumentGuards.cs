namespace PharmacyStock.Core.Documents;

/// <summary>Validates calculation inputs.</summary>
internal static class DocumentGuards
{
    /// <summary>Validates the document identifier and patient details.</summary>
    /// <param name="documentId">Positive document identifier.</param>
    /// <param name="patientName">Nonblank patient name.</param>
    /// <param name="patientEmail">Patient email containing the at sign.</param>
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

    /// <summary>Validates one medicine line.</summary>
    /// <param name="line">Medicine line.</param>
    public static void EnsureLineValid(PrescriptionLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        ArgumentException.ThrowIfNullOrWhiteSpace(line.MedicineCode);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(line.Quantity, nameof(line.Quantity));
        EnsureAmountValid(line.UnitPrice, nameof(line.UnitPrice));
    }

    /// <summary>Validates a nonempty collection of medicine lines.</summary>
    /// <param name="lines">Medicine lines.</param>
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

    /// <summary>Rejects undefined enumeration values.</summary>
    /// <param name="status">Supported document state.</param>
    public static void EnsureStatusValid(DocumentStatus status)
    {
        if (!Enum.IsDefined(status))
        {
            throw new ArgumentOutOfRangeException(nameof(status), status, "Невідомий стан.");
        }
    }

    /// <summary>Validates all request fields before calculations.</summary>
    /// <param name="request">Calculation request.</param>
    public static void EnsureRequestValid(TotalRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Patient);
        EnsureHeaderValid(request.DocumentId, request.Patient.Name, request.Patient.Email);
        EnsureLinesValid(request.Lines);
        EnsureStatusValid(request.Status);
        EnsureAmountValid(request.DeliveryPrice, nameof(request.DeliveryPrice));
    }

    /// <summary>Rejects negative amounts while accepting both decimal zero encodings.</summary>
    /// <param name="amount">Amount to validate.</param>
    /// <param name="parameterName">Name of the original argument.</param>
    public static void EnsureAmountValid(decimal amount, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(parameterName);
        ArgumentOutOfRangeException.ThrowIfNegative(amount == 0m ? 0m : amount, parameterName);
    }
}
