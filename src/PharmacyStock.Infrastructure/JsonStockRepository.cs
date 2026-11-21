using System.Text.Json;
using PharmacyStock.Core.Abstractions;
using PharmacyStock.Core.Application;
using PharmacyStock.Core.Domain;
namespace PharmacyStock.Infrastructure;
/// <summary>Локальний JSON-стан; змінює пам'ять лише після повної перевірки знімка.</summary>
public sealed partial class JsonStockRepository : IStockRepository
{
    private readonly string _path;
    private List<Medicine> _medicines = new();
    private List<StockBatch> _batches = new();
    private List<Prescription> _prescriptions = new();
    /// <summary>Встановлює шлях без неявного читання чи запису.</summary>
    /// <param name="path">Файл з умовними даними.</param>
    public JsonStockRepository(string path) => _path = path;
    /// <inheritdoc/>
    public IReadOnlyList<Medicine> Medicines => _medicines.AsReadOnly();
    /// <inheritdoc/>
    public IReadOnlyList<StockBatch> Batches => _batches.AsReadOnly();
    /// <inheritdoc/>
    public IReadOnlyList<Prescription> Prescriptions => _prescriptions.AsReadOnly();
    /// <inheritdoc/>
    public void AddBatch(StockBatch batch) => _batches.Add(batch);
    /// <inheritdoc/>
    public void AddPrescription(Prescription prescription) => _prescriptions.Add(prescription);
    /// <inheritdoc/>
    public void Save()
    {
        var snapshot = new Snapshot(_medicines.ToArray(), _batches.ToArray(), _prescriptions.Select(ToData).ToArray());
        string temporary = _path + ".tmp";
        File.WriteAllText(temporary, JsonSerializer.Serialize(snapshot));
        File.Move(temporary, _path, true);
    }
    /// <inheritdoc/>
    /// <exception cref="InvalidDataException">Знімок не задовольняє правилам даних.</exception>
    public void Load()
    {
        Snapshot snapshot = JsonSerializer.Deserialize<Snapshot>(File.ReadAllText(_path))
            ?? throw new InvalidDataException("Порожній JSON-знімок.");
        Validate(snapshot);
        List<Prescription> prescriptions = snapshot.Prescriptions.Select(FromData).ToList();
        _medicines = snapshot.Medicines.ToList();
        _batches = snapshot.Batches.ToList();
        _prescriptions = prescriptions;
    }
    private static PrescriptionData ToData(Prescription prescription) => new(
        prescription.Id, prescription.PatientId, prescription.CreatedAt, prescription.Status, prescription.Lines.ToArray());
    private static Prescription FromData(PrescriptionData data)
    {
        var prescription = new Prescription(data.Id, data.PatientId, data.CreatedAt);
        foreach (PrescriptionLine line in data.Lines)
        {
            prescription.AddLine(line);
        }
        RestoreStatus(prescription, data.Status);
        return prescription;
    }
    private static void RestoreStatus(Prescription prescription, PrescriptionStatus status)
    {
        switch (status)
        {
            case PrescriptionStatus.Approved: prescription.Approve(); break;
            case PrescriptionStatus.Dispensed: prescription.Approve(); prescription.Dispense(); break;
            case PrescriptionStatus.Cancelled: prescription.Cancel(); break;
            case PrescriptionStatus.Expired: prescription.Expire(); break;
        }
    }
    private sealed record Snapshot(Medicine[] Medicines, StockBatch[] Batches, PrescriptionData[] Prescriptions);
    private sealed record PrescriptionData(
        int Id, int PatientId, DateOnly CreatedAt, PrescriptionStatus Status, PrescriptionLine[] Lines);
}
