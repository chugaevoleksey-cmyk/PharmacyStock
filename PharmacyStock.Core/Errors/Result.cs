namespace PharmacyStock.Core.Errors;

/// <summary>Contains a value or an expected input failure.</summary>
/// <typeparam name="T">Type of the successful value.</typeparam>
public readonly struct Result<T>
    where T : notnull
{
    private readonly T? _value;
    private readonly string? _error;

    private Result(bool success, T? value, string? error)
    {
        IsSuccess = success;
        _value = value;
        _error = error;
    }

    /// <summary>Gets a value indicating whether a valid value is available.</summary>
    public bool IsSuccess { get; }

    /// <summary>Gets the value only when the result is successful.</summary>
    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Спочатку перевірте IsSuccess.");

    /// <summary>Gets the failure explanation, or an empty string on success.</summary>
    public string Error => IsSuccess
        ? string.Empty
        : _error ?? "Результат не ініціалізовано.";

    /// <summary>Creates a successful result.</summary>
    /// <param name="value">Non-null value.</param>
    /// <returns>A successful result.</returns>
    public static Result<T> Ok(T value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new Result<T>(true, value, null);
    }

    /// <summary>Creates an expected failure result.</summary>
    /// <param name="error">Nonblank explanation.</param>
    /// <returns>A failed result.</returns>
    public static Result<T> Fail(string error)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(error);
        return new Result<T>(false, default, error);
    }
}
