namespace PharmacyStock.Core.Legacy;

public interface IPrescriptionStore
{
    void Add(LegacyPrescription prescription);
    LegacyPrescription GetById(int id);
    void SaveToFile(LegacyPrescription prescription, string path);
    void SendEmail(string to, string body);
    string ExportCsv();
    void Backup(string folder);
    void PrintInvoice(LegacyPrescription prescription);
}
