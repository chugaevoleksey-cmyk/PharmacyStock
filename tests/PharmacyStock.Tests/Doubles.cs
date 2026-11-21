using PharmacyStock.Core.Abstractions;
using PharmacyStock.Core.Application;
using PharmacyStock.Core.Domain;
namespace PharmacyStock.Tests;

internal sealed class FakeClock : IClock
{
    public DateOnly Today { get; set; } = new(2026, 11, 21);
}
internal sealed class FakeLog : IAppLog
{
    public List<string> Events { get; } = new();
    public void Write(string level, string scenario, string message) => Events.Add($"{level}|{scenario}|{message}");
}
internal sealed class FakeRepository : IStockRepository
{
    public List<Medicine> Catalog { get; } = new()
    {
        new("RX", "Ліки Rx", 12.345m, new MedicineRules(true, 10)),
        new("OTC", "Ліки без рецепта", 5.005m, new MedicineRules(false, 10)),
    };
    public List<StockBatch> Stock { get; } = new();
    public List<Prescription> Documents { get; } = new();
    public IReadOnlyList<Medicine> Medicines => Catalog;
    public IReadOnlyList<StockBatch> Batches => Stock;
    public IReadOnlyList<Prescription> Prescriptions => Documents;
    public void AddBatch(StockBatch batch) => Stock.Add(batch);
    public void AddPrescription(Prescription prescription) => Documents.Add(prescription);
    public void Load() => throw new NotSupportedException("Фейк не читає файлів.");
    public void Save() => throw new NotSupportedException("Фейк не пише файлів.");
}
internal sealed class Scenario
{
    public FakeClock Clock { get; } = new();
    public FakeRepository Repository { get; } = new();
    public FakeLog Log { get; } = new();
    public StockService Service => new(Repository, Clock, new DispensingPolicy(), Log);
    public Prescription Prescription(int age = 0, string code = "RX", int units = 10)
    {
        var prescription = new Prescription(1, 1, Clock.Today.AddDays(-age));
        prescription.AddLine(new PrescriptionLine(code, units, 12.345m));
        prescription.Approve();
        Repository.AddPrescription(prescription);
        return prescription;
    }
    public void Batch(int units = 20, int days = 7, string code = "RX") => Repository.AddBatch(
        new StockBatch(Repository.Batches.Count + 1, code, units, Clock.Today.AddDays(days)));
}
