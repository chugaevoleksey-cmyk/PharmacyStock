namespace PharmacyStock.Core.Domain;

/// <summary>Represents the Patient type.</summary>
public sealed record Patient
{
    /// <summary>Initializes a new instance of the <see cref="Patient"/> class.</summary>
    /// <param name="id">The id argument.</param>
    /// <param name="name">The name argument.</param>
    /// <param name="email">The email argument.</param>
    /// <param name="isRegular">The isRegular argument.</param>
    public Patient(int id, string name, string email, bool isRegular = false)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        if (!email.Contains('@'))
        {
            throw new ArgumentException("Пошта має містити @.", nameof(email));
        }

        Id = id;
        Name = name;
        Email = email;
        IsRegular = isRegular;
    }

    /// <summary>Gets the Id value.</summary>
    public int Id { get; }

    /// <summary>Gets the Name value.</summary>
    public string Name { get; }

    /// <summary>Gets the Email value.</summary>
    public string Email { get; }

    /// <summary>Gets a value indicating whether IsRegular applies.</summary>
    public bool IsRegular { get; }
}
