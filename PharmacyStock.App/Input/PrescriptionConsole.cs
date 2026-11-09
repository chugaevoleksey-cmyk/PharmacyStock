using PharmacyStock.Core.Domain;
using PharmacyStock.Core.Errors;
using PharmacyStock.Core.Services;

namespace PharmacyStock.App.Input;

/// <summary>Represents the PrescriptionConsole type.</summary>
public sealed class PrescriptionConsole
{
    private readonly TextReader _input;
    private readonly TextWriter _output;

    /// <summary>Initializes a new instance of the <see cref="PrescriptionConsole"/> class.</summary>
    /// <param name="input">The input argument.</param>
    /// <param name="output">The output argument.</param>
    public PrescriptionConsole(TextReader input, TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(output);
        _input = input;
        _output = output;
    }

    /// <summary>Performs the ReadAndAddLine operation.</summary>
    /// <param name="service">The service argument.</param>
    /// <param name="prescriptionId">The prescriptionId argument.</param>
    /// <returns>The result of the operation.</returns>
    public bool ReadAndAddLine(PrescriptionService service, int prescriptionId)
    {
        ArgumentNullException.ThrowIfNull(service);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(prescriptionId);
        while (true)
        {
            _output.WriteLine("Код препарату:");
            string? code = _input.ReadLine();
            _output.WriteLine("Кількість:");
            string? quantity = _input.ReadLine();
            _output.WriteLine("Ціна за одиницю (крапка):");
            string? price = _input.ReadLine();
            if (code is null || quantity is null || price is null)
            {
                _output.WriteLine("Ввід завершено.");
                return false;
            }

            Result<PrescriptionLine> parsed = PrescriptionLineParser.Parse(code, quantity, price);
            if (!parsed.IsSuccess)
            {
                _output.WriteLine("Помилка вводу: " + parsed.Error);
                continue;
            }

            service.AddLine(prescriptionId, parsed.Value);
            return true;
        }
    }
}
