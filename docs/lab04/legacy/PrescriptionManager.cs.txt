namespace PharmacyStock.Core.Legacy;

public class PrescriptionManager
{
    private readonly FilePrescriptionStore _store = new FilePrescriptionStore();
    private readonly SmtpMailer _mailer = new SmtpMailer();

    public decimal Place(LegacyPrescription prescription, string patientType)
    {
        decimal total = 0m;
        foreach (var line in prescription.Lines)
            total += line.UnitPrice * line.Quantity;

        if (patientType == "regular")
            total = total * 0.95m;
        else if (patientType == "vip")
            total = total * 0.90m;
        else if (patientType == "staff")
            total = total * 0.70m;

        if (total > 1000m)
            total = total - 50m;

        prescription.Total = total;
        prescription.Status = 1;
        _store.SaveToFile(prescription, "prescriptions.json");
        _mailer.Send(prescription.Email, "Prescription " + prescription.Id, "" + total);
        return total;
    }

    public string BuildCsvReport(List<LegacyPrescription> prescriptions)
    {
        var text = "id;total;status" + Environment.NewLine;
        foreach (var prescription in prescriptions)
            text += prescription.Id + ";" + prescription.Total + ";" + prescription.Status;

        File.WriteAllText("report.csv", text);
        Console.WriteLine("Звіт збережено");
        return text;
    }

    public void Archive(LegacyPrescription prescription)
    {
        LegacyPrescription archived = new ArchivedPrescription(prescription.Id);
        foreach (var line in prescription.Lines)
            archived.AddLine(line);

        _store.SaveToFile(archived, "archive.json");
    }
}
