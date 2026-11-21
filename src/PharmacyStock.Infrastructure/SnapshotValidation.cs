using PharmacyStock.Core.Application;
using PharmacyStock.Core.Domain;
namespace PharmacyStock.Infrastructure;

public sealed partial class JsonStockRepository
{
    private static void Validate(Snapshot snapshot)
    {
        if (snapshot.Medicines is null || snapshot.Batches is null || snapshot.Prescriptions is null)
        {
            throw new InvalidDataException("У знімку бракує колекцій.");
        }
        ValidateCatalog(snapshot.Medicines);
        var codes = snapshot.Medicines.Select(x => x.Code).ToHashSet(StringComparer.Ordinal);
        ValidateBatches(snapshot.Batches, codes);
        ValidatePrescriptions(snapshot.Prescriptions, codes);
    }
    private static void ValidateCatalog(Medicine[] medicines)
    {
        var codes = new HashSet<string>(StringComparer.Ordinal);
        foreach (Medicine medicine in medicines)
        {
            if (medicine is null || !codes.Add(medicine.Code))
            {
                throw new InvalidDataException("Некоректний каталог.");
            }
            if (InvalidMedicine(medicine))
            {
                throw new InvalidDataException("Некоректні код ціна або правила.");
            }
            if (medicine.Rules.MaximumUnits is <= 0 or > InputValidation.MaximumQuantity)
            {
                throw new InvalidDataException("Некоректний ліміт препарату.");
            }
        }
    }
    private static bool InvalidMedicine(Medicine medicine) => string.IsNullOrWhiteSpace(medicine.Code)
        || string.IsNullOrWhiteSpace(medicine.Name) || medicine.Price <= 0 || medicine.Price > InputValidation.MaximumPrice || medicine.Rules is null;

    private static void ValidateBatches(StockBatch[] batches, HashSet<string> codes)
    {
        var ids = new HashSet<int>();
        foreach (StockBatch batch in batches)
        {
            if (batch is null || batch.Id <= 0 || !ids.Add(batch.Id))
            {
                throw new InvalidDataException("Некоректний номер партії.");
            }
            if (!codes.Contains(batch.MedicineCode) || batch.Quantity is < 0 or > InputValidation.MaximumQuantity)
            {
                throw new InvalidDataException("Некоректний залишок або код партії.");
            }
        }
    }
    private static void ValidatePrescriptions(PrescriptionData[] prescriptions, HashSet<string> codes)
    {
        var ids = new HashSet<int>();
        foreach (PrescriptionData data in prescriptions)
        {
            if (data is null || data.Id <= 0 || !ids.Add(data.Id) || data.PatientId <= 0)
            {
                throw new InvalidDataException("Некоректні реквізити рецепта.");
            }
            ValidateLines(data, codes);
        }
    }
    private static bool InvalidDocument(PrescriptionData data) => !Enum.IsDefined(data.Status)
        || data.Lines is null || data.Lines.Length > 1;

    private static bool InvalidLine(PrescriptionLine line, HashSet<string> codes) => line is null
        || !codes.Contains(line.MedicineCode) || !InputValidation.Line(line.MedicineCode, line.Quantity).IsValid;

    private static void ValidateLines(PrescriptionData data, HashSet<string> codes)
    {
        if (InvalidDocument(data))
        {
            throw new InvalidDataException("Некоректний стан або більше однієї позиції.");
        }
        if (data.Lines.Length == 0 && data.Status is PrescriptionStatus.Approved or PrescriptionStatus.Dispensed)
        {
            throw new InvalidDataException("Підтверджений рецепт порожній.");
        }
        foreach (PrescriptionLine line in data.Lines)
        {
            if (InvalidLine(line, codes))
            {
                throw new InvalidDataException("Некоректна позиція рецепта.");
            }
        }
    }
}
