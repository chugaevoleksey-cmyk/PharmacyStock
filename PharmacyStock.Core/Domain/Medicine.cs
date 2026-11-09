namespace PharmacyStock.Core.Domain;

/// <summary>Represents the Medicine type.</summary>
public sealed class Medicine
{
    /// <summary>Initializes a new instance of the <see cref="Medicine"/> class.</summary>
    /// <param name="code">The code argument.</param>
    /// <param name="name">The name argument.</param>
    /// <param name="price">The price argument.</param>
    /// <param name="stock">The stock argument.</param>
    public Medicine(string code, string name, decimal price, int stock)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);
        ArgumentOutOfRangeException.ThrowIfNegative(stock);
        Code = code;
        Name = name;
        Price = price;
        Stock = stock;
    }

    /// <summary>Gets the Code value.</summary>
    public string Code { get; }

    /// <summary>Gets the Name value.</summary>
    public string Name { get; }

    /// <summary>Gets the Price value.</summary>
    public decimal Price { get; }

    /// <summary>Gets the Stock value.</summary>
    public int Stock { get; private set; }

    /// <summary>Performs the Restock operation.</summary>
    /// <param name="quantity">The quantity argument.</param>
    public void Restock(int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        Stock = checked(Stock + quantity);
    }

    /// <summary>Performs the DecreaseStock operation.</summary>
    /// <param name="quantity">The quantity argument.</param>
    public void DecreaseStock(int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        if (quantity > Stock)
        {
            throw new InvalidOperationException("Недостатньо препарату на складі.");
        }

        Stock -= quantity;
    }
}
