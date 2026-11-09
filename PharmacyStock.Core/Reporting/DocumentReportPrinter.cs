namespace PharmacyStock.Core.Reporting;

/// <summary>Writes an existing report to the supplied writer.</summary>
public sealed class DocumentReportPrinter
{
    private readonly TextWriter _output;

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentReportPrinter"/> class.
    /// </summary>
    /// <param name="output">Writer supplied and owned by the caller.</param>
    public DocumentReportPrinter(TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(output);
        _output = output;
    }

    /// <summary>Writes the supplied report without taking ownership of the writer.</summary>
    /// <param name="report">Nonempty report text.</param>
    public void Print(string report)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(report);
        _output.Write(report);
    }
}
