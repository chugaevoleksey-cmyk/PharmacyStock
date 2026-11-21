namespace PharmacyStock.Core.Domain;

/// <summary>Represents the Patient type.</summary>
public sealed record Patient
{
    /// <summary>Initializes a new instance of the <see cref="Patient"/> class.</summary>
    /// <param name="id">Positive identifier.</param>
    /// <param name="name">Nonblank display name.</param>
    /// <param name="email">Nonblank email containing the at sign.</param>
    /// <param name="isRegular">Whether the patient is marked as regular.</param>
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
