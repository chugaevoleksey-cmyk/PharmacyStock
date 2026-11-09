namespace PharmacyStock.Core.Legacy;

/// <summary>Represents the Prescription type.</summary>
public class Prescription
{
    private readonly List<string[]> _lines = new();

    /// <summary>Represents a pharmacy prescription.</summary>
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
    public int Status { get; private set; }

    /// <summary>Gets the document creation time.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>Adds a medicine line to the prescription.</summary>
    /// <param name="medicineCode">Medicine identifier.</param>
    /// <param name="quantity">Number of units.</param>
    /// <param name="unitPrice">Price for one unit.</param>
    public void AddLine(string medicineCode, int quantity, decimal unitPrice)
    {
        string[] line = new string[3];
        line[0] = medicineCode;
        line[1] = quantity.ToString();
        line[2] = unitPrice.ToString();
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
            int quantity = int.Parse(_lines[i][1]);
            decimal unitPrice = decimal.Parse(_lines[i][2]);
            total = total + quantity * unitPrice;
            lineCount = lineCount + 1;
        }

        // Порядок знижок успадковано; регулярна знижка має пріоритет.
        if (isRegularPatient == true && total > 1000)
        {
            total = total * 0.9m;
        }
        else if (total > 5000)
        {
            total = total * 0.85m;
        }
        else
        {
            total = total;
        }

        if (lineCount > 10)
        {
            total = total - 100;
        }

        if (total < 0)
        {
            total = 0;
        }

        // ПДВ нараховується після всіх знижок, як у початковому коді.
        total = total + total * 0.2m;
        return Math.Round(total, 2);
    }

    /// <summary>Changes status only when the inherited transition is allowed.</summary>
    /// <param name="newStatus">Requested target status.</param>
    /// <returns>True if status changed; false if the transition was rejected.</returns>
    public bool TryChangeStatus(int newStatus)
    {
        if (Status == 0 && newStatus == 1)
        {
            Status = 1;
            return true;
        }

        if (Status == 1 && newStatus == 2)
        {
            Status = 2;
            return true;
        }

        if (Status == 0 && newStatus == 3)
        {
            Status = 3;
            return true;
        }

        return false;
    }

    /// <summary>Checks the inherited document validity conditions.</summary>
    /// <returns>True if the inherited validity conditions hold; otherwise false.</returns>
    public bool IsValid()
    {
        if (Id != null && Id != "" && PatientName != null && PatientName.Length > 2
            && _lines.Count > 0 && _lines.Count < 100 && Status >= 0 && Status <= 3)
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
        string report = "";
        for (int i = 0; i < _lines.Count; i++)
        {
            report = report + "Товар: " + _lines[i][0] + "; кількість: " + _lines[i][1]
                + "; ціна: " + _lines[i][2] + "; сума: "
                + (int.Parse(_lines[i][1]) * decimal.Parse(_lines[i][2])) + "\n";
        }

        report = report + "Разом: " + CalculateTotal(false) + "\n";
        return report;
    }

    /// <summary>Finds the first prescription with the specified identifier.</summary>
    /// <param name="prescriptions">Prescriptions to search.</param>
    /// <param name="prescriptionId">Identifier to find.</param>
    /// <returns>The first matching prescription, or null if none was found.</returns>
    public static Prescription FindById(List<Prescription> prescriptions, string prescriptionId)
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
}
