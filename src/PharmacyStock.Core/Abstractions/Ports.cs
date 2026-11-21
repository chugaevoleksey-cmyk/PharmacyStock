using PharmacyStock.Core.Domain;
namespace PharmacyStock.Core.Abstractions;
/// <summary>Дата правил; підмінюється фіксованою датою в тестах.</summary>
public interface IClock
{
    /// <summary>Локальна дата робочої зміни.</summary>
    DateOnly Today { get; }
}
/// <summary>Збереження робочого стану без прив'язки логіки до формату файла.</summary>
public interface IStockRepository
{
    /// <summary>Каталог, доступний для читання.</summary>
    IReadOnlyList<Medicine> Medicines { get; }
    /// <summary>Поточні партії з живими залишками.</summary>
    IReadOnlyList<StockBatch> Batches { get; }
    /// <summary>Документи поточної зміни.</summary>
    IReadOnlyList<Prescription> Prescriptions { get; }
    /// <summary>Додає перевірену партію.</summary>
    /// <param name="batch">Партія з новим номером.</param>
    void AddBatch(StockBatch batch);
    /// <summary>Додає перевірений документ.</summary>
    /// <param name="prescription">Рецепт із новим номером.</param>
    void AddPrescription(Prescription prescription);
    /// <summary>Завантажує повністю перевірений знімок.</summary>
    void Load();
    /// <summary>Зберігає поточний стан; помилка не приховується.</summary>
    void Save();
}
/// <summary>Подія журналу без вводу користувача або персональних даних.</summary>
public interface IAppLog
{
    /// <summary>Записує рівень, сценарій та контрольований код події.</summary>
    /// <param name="level">Information, Warning або Error.</param>
    /// <param name="scenario">Ідентифікатор UC.</param>
    /// <param name="message">Контрольований текст, не сирий ввід.</param>
    void Write(string level, string scenario, string message);
}
