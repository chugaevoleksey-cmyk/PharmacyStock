namespace PharmacyStock.Core.Legacy;

public class Prescription
{
    private readonly List<string[]> _lines = new();

    public Prescription(string id, string patientName)
    {
        Id = id; // ставимо id
        PatientName = patientName;
        CreatedAt = DateTime.Now;
    }

    // public string prim;

    public string Id { get; private set; }

    public string PatientName { get; private set; }

    public int Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public void AddLine(string medicineCode, int quantity, decimal unitPrice)
    {
        string[] line = new string[3];
        line[0] = medicineCode;
        line[1] = quantity.ToString();
        line[2] = unitPrice.ToString();
        _lines.Add(line); // додаємо позицію
    }

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

        total = total + total * 0.2m;
        return Math.Round(total, 2);
    }

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
