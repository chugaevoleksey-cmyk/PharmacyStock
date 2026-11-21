using System.Text.Json;
using System.Text.Json.Nodes;
using PharmacyStock.Core.Application;
using PharmacyStock.Core.Domain;
using PharmacyStock.Infrastructure;
using Xunit;
namespace PharmacyStock.Tests;

public sealed class StoreTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
    public StoreTests() => Directory.CreateDirectory(_directory);
    public void Dispose() => Directory.Delete(_directory, true);
    private string FilePath => Path.Combine(_directory, "stock.json");
    private JsonStockRepository Seed()
    {
        File.WriteAllText(FilePath, """
            {"Medicines":[{"Code":"RX","Name":"Умовний","Price":10,
            "Rules":{"RequiresPrescription":true,"MaximumUnits":10}}],
            "Batches":[],"Prescriptions":[]}
            """);
        var repository = new JsonStockRepository(FilePath);
        repository.Load();
        return repository;
    }
    [Theory]
    [InlineData(PrescriptionStatus.New)]
    [InlineData(PrescriptionStatus.Approved)]
    [InlineData(PrescriptionStatus.Dispensed)]
    [InlineData(PrescriptionStatus.Cancelled)]
    [InlineData(PrescriptionStatus.Expired)]
    public void Store_AllStates_RoundTrips(PrescriptionStatus status)
    {
        JsonStockRepository repository = Seed();
        var prescription = new Prescription(19, 1, new DateOnly(2026, 11, 21));
        prescription.AddLine(new PrescriptionLine("RX", 2, 10m));
        SetStatus(prescription, status);
        repository.AddPrescription(prescription);
        repository.AddBatch(new StockBatch(1, "RX", 8, new DateOnly(2026, 12, 1)));
        repository.Save();

        var loaded = new JsonStockRepository(FilePath);
        loaded.Load();

        Assert.Equal(status, Assert.Single(loaded.Prescriptions).Status);
        Assert.Equal(2, loaded.Prescriptions[0].Lines[0].Quantity);
        Assert.Equal(8, Assert.Single(loaded.Batches).Quantity);
    }
    private static void SetStatus(Prescription prescription, PrescriptionStatus status)
    {
        switch (status)
        {
            case PrescriptionStatus.Approved: prescription.Approve(); break;
            case PrescriptionStatus.Dispensed: prescription.Approve(); prescription.Dispense(); break;
            case PrescriptionStatus.Cancelled: prescription.Cancel(); break;
            case PrescriptionStatus.Expired: prescription.Expire(); break;
        }
    }
    [Theory]
    [InlineData("null")]
    [InlineData("{}")]
    [InlineData("{broken")]
    public void Store_InvalidJson_PreservesWorkingState(string content)
    {
        JsonStockRepository repository = Seed();
        File.WriteAllText(FilePath, content);
        Assert.ThrowsAny<Exception>(repository.Load);
        Assert.Single(repository.Medicines);
        Assert.Equal(content, File.ReadAllText(FilePath));
    }
    [Fact]
    public void Store_UnknownBatchCode_PreservesStateAndFile()
    {
        JsonStockRepository repository = Seed();
        string content = File.ReadAllText(FilePath).Replace("Batches\":[]", "Batches\":[{\"Id\":1,\"MedicineCode\":\"?\",\"Quantity\":1,\"ExpiresOn\":\"2026-11-21\"}]", StringComparison.Ordinal);
        File.WriteAllText(FilePath, content);
        Assert.Throws<InvalidDataException>(repository.Load);
        Assert.Empty(repository.Batches);
        Assert.Equal(content, File.ReadAllText(FilePath));
    }
    [Fact]
    public void Store_MissingFile_DoesNotOverwriteAnything()
    {
        Assert.Throws<FileNotFoundException>(() => new JsonStockRepository(FilePath).Load());
        Assert.False(File.Exists(FilePath));
    }
    [Fact]
    public void Store_InvalidDirectory_SaveFailsLoudly()
    {
        var repository = new JsonStockRepository(Path.Combine(_directory, "missing", "stock.json"));
        Assert.Throws<DirectoryNotFoundException>(repository.Save);
    }
    [Fact]
    public void Log_ThreeLevels_RecordsContext()
    {
        string path = Path.Combine(_directory, "app.log");
        var log = new FileAppLog(path);
        log.Write("Information", "UC-01", "Початок");
        log.Write("Warning", "UC-01", "Відхилено");
        log.Write("Error", "UC-06", "IOException");
        string[] lines = File.ReadAllLines(path);
        Assert.Equal(3, lines.Length);
        Assert.Contains("|Error|UC-06|IOException", lines[2], StringComparison.Ordinal);
    }
}
