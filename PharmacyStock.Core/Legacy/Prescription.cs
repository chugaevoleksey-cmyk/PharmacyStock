using System.Globalization;

namespace PharmacyStock.Core.Legacy;

/// <summary>Represents the Prescription type.</summary>
public class Prescription
{
    /// <summary>Inherited VAT rate.</summary>
    private const decimal VatRate = 0.2m;

    /// <summary>Regular discount threshold.</summary>
    private const decimal RegularDiscountThreshold = 1000m;

    /// <summary>Regular discount fraction.</summary>
    private const decimal RegularDiscountRate = 0.1m;

    /// <summary>Large prescription threshold.</summary>
    private const decimal LargePrescriptionThreshold = 5000m;

    /// <summary>Large prescription discount fraction.</summary>
    private const decimal LargePrescriptionDiscountRate = 0.15m;

    /// <summary>Line count required for bulk discount.</summary>
    private const int BulkLineCount = 10;

    /// <summary>Bulk discount amount.</summary>
    private const decimal BulkDiscountAmount = 100m;

    /// <summary>Exclusive upper limit of the line count.</summary>
    private const int MaxLineCount = 100;

    /// <summary>Minimum patient name length.</summary>
    private const int MinPatientNameLength = 3;

    /// <summary>New document status.</summary>
    private const int StatusNew = 0;

    /// <summary>Approved document status.</summary>
    private const int StatusApproved = 1;

    /// <summary>Dispensed document status.</summary>
    private const int StatusDispensed = 2;

    /// <summary>Cancelled document status.</summary>
    private const int StatusCancelled = 3;

    private readonly List<string[]> _lines = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Prescription"/> class.
    /// </summary>
    /// <param name="id">Document identifier.</param>
    /// <param name="patientName">Patient display name.</param>
    public Prescription(string id, string patientName)
    {
        Id = id;
        PatientName = patientName;
        CreatedAt = DateTime.Now;
    }

    /// <summary>Gets the document or patient identifier.</summary>
    public string Id { get; private set; }

    /// <summary>Gets the patient name.</summary>
    public string PatientName { get; private set; }

    /// <summary>Gets the current document status.</summary>
    public int Status { get; private set; } = StatusNew;

    /// <summary>Gets the document creation time.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>Finds the first prescription with the specified identifier.</summary>
    /// <param name="prescriptions">Prescriptions to search.</param>
    /// <param name="prescriptionId">Identifier to find.</param>
    /// <returns>The first matching prescription, or null if none was found.</returns>
    public static Prescription? FindById(List<Prescription> prescriptions, string prescriptionId)
    {
        for (int i = 0; i < prescriptions.Count; i++)
        {
            if (prescriptions[i].Id == prescriptionId)
            {
                return prescriptions[i];
            }
        }

        return null;
    }

    /// <summary>Adds a medicine line to the prescription.</summary>
    /// <param name="medicineCode">Medicine identifier.</param>
    /// <param name="quantity">Number of units.</param>
    /// <param name="unitPrice">Price for one unit.</param>
    public void AddLine(string medicineCode, int quantity, decimal unitPrice)
    {
        string[] line = new string[3];
        line[0] = medicineCode;
        line[1] = quantity.ToString(CultureInfo.CurrentCulture);
        line[2] = unitPrice.ToString(CultureInfo.CurrentCulture);
        _lines.Add(line);
    }

    /// <summary>Calculates the inherited total including discounts and VAT.</summary>
    /// <param name="isRegularPatient">Whether the regular-patient discount is requested.</param>
    /// <returns>The total rounded to two decimal places.</returns>
    public decimal CalculateTotal(bool isRegularPatient)
    {
        decimal total = 0;
        int lineCount = 0;
        for (int i = 0; i < _lines.Count; i++)
        {
            int quantity = int.Parse(_lines[i][1], CultureInfo.CurrentCulture);
            decimal unitPrice = decimal.Parse(_lines[i][2], CultureInfo.CurrentCulture);
            total = total + (quantity * unitPrice);
            lineCount = lineCount + 1;
        }

        // Порядок знижок успадковано; регулярна знижка має пріоритет.
        if (isRegularPatient == true && total > RegularDiscountThreshold)
        {
            total = total * (1m - RegularDiscountRate);
        }
        else if (total > LargePrescriptionThreshold)
        {
            total = total * (1m - LargePrescriptionDiscountRate);
        }

        if (lineCount > BulkLineCount)
        {
            total = total - BulkDiscountAmount;
        }

        if (total < 0)
        {
            total = 0;
        }

        // ПДВ нараховується після всіх знижок, як у початковому коді.
        total = total + (total * VatRate);
        return Math.Round(total, 2);
    }

    /// <summary>Changes status only when the inherited transition is allowed.</summary>
    /// <param name="newStatus">Requested target status.</param>
    /// <returns>True if status changed; false if the transition was rejected.</returns>
    public bool TryChangeStatus(int newStatus)
    {
        if (Status == StatusNew && newStatus == StatusApproved)
        {
            Status = StatusApproved;
            return true;
        }

        if (Status == StatusApproved && newStatus == StatusDispensed)
        {
            Status = StatusDispensed;
            return true;
        }

        if (Status == StatusNew && newStatus == StatusCancelled)
        {
            Status = StatusCancelled;
            return true;
        }

        return false;
    }

    /// <summary>Checks the inherited document validity conditions.</summary>
    /// <returns>True if the inherited validity conditions hold; otherwise false.</returns>
    public bool IsValid()
    {
        if (Id != null && Id != string.Empty
            && PatientName != null
            && PatientName.Length >= MinPatientNameLength
            && _lines.Count > 0
            && _lines.Count < MaxLineCount
            && Status >= StatusNew
            && Status <= StatusCancelled)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>Builds the inherited text report without changing its format.</summary>
    /// <returns>The report with the original labels and line endings.</returns>
    public string BuildReport()
    {
        string report = string.Empty;
        for (int i = 0; i < _lines.Count; i++)
        {
            report = report + "Товар: " + _lines[i][0] + "; кількість: " + _lines[i][1]
                + "; ціна: " + _lines[i][2] + "; сума: "
                + (int.Parse(_lines[i][1], CultureInfo.CurrentCulture)
                    * decimal.Parse(_lines[i][2], CultureInfo.CurrentCulture)) + "\n";
        }

        report = report + "Разом: " + CalculateTotal(false) + "\n";
        return report;
    }
}
