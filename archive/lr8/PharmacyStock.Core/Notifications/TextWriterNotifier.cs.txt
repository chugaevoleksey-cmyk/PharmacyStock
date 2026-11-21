using System.Globalization;
using PharmacyStock.Core.Abstractions;

namespace PharmacyStock.Core.Notifications;

/// <summary>Represents the TextWriterNotifier type.</summary>
public sealed class TextWriterNotifier : INotifier
{
    private readonly TextWriter _output;

    /// <summary>Initializes a new instance of the <see cref="TextWriterNotifier"/> class.</summary>
    /// <param name="output">Writer owned and disposed by the caller.</param>
    public TextWriterNotifier(TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(output);
        _output = output;
    }

    /// <summary>Announces a stored prescription without prescribing a transport.</summary>
    /// <param name="prescriptionId">Positive prescription identifier.</param>
    /// <param name="total">Nonnegative quoted total.</param>
    public void Notify(int prescriptionId, decimal total)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(prescriptionId);
        ArgumentOutOfRangeException.ThrowIfLessThan(total, 0m);
        _output.WriteLine(string.Format(
            CultureInfo.InvariantCulture,
            "Збережено рецепт № {0}. Сума: {1:F2}",
            prescriptionId,
            total));
    }
}
