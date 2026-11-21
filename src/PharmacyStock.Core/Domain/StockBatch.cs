namespace PharmacyStock.Core.Domain;
/// <summary>Партія з незмінними реквізитами та змінним залишком.</summary>
public sealed class StockBatch
{
    /// <summary>Створює партію після перевірки даних на прикладній межі.</summary>
    /// <param name="id">Унікальний номер партії.</param>
    /// <param name="medicineCode">Код із каталогу.</param>
    /// <param name="quantity">Невід'ємний залишок.</param>
    /// <param name="expiresOn">Остання допустима дата відпуску включно.</param>
    public StockBatch(int id, string medicineCode, int quantity, DateOnly expiresOn)
    {
        Id = id;
        MedicineCode = medicineCode;
        Quantity = quantity;
        ExpiresOn = expiresOn;
    }
    /// <summary>Номер партії для відстеження й стабільного сортування.</summary>
    public int Id { get; }
    /// <summary>Ключ препарату в каталозі.</summary>
    public string MedicineCode { get; }
    /// <summary>Кількість одиниць, доступних у цій партії.</summary>
    public int Quantity { get; private set; }
    /// <summary>Остання дата використання включно.</summary>
    public DateOnly ExpiresOn { get; }
    /// <summary>Списує одиниці за вже перевіреним планом відпуску.</summary>
    /// <param name="quantity">Кількість від 1 до залишку.</param>
    /// <exception cref="InvalidOperationException">План порушує інваріант залишку.</exception>
    public void Deduct(int quantity)
    {
        if (quantity <= 0 || quantity > Quantity)
        {
            throw new InvalidOperationException("Некоректний план списання.");
        }
        Quantity -= quantity;
    }
}
