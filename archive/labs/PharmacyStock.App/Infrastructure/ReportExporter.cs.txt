using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging;
using PharmacyStock.Core.Domain;
using PharmacyStock.Core.Logging;
using PharmacyStock.Core.Services;

namespace PharmacyStock.App.Infrastructure;

/// <summary>Represents the ReportExporter type.</summary>
public sealed class ReportExporter
{
    private readonly PrescriptionService _service;
    private readonly ILogger<ReportExporter> _log;

    /// <summary>Initializes a new instance of the <see cref="ReportExporter"/> class.</summary>
    /// <param name="service">The service argument.</param>
    /// <param name="log">The log argument.</param>
    public ReportExporter(PrescriptionService service, ILogger<ReportExporter> log)
    {
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(log);
        _service = service;
        _log = log;
    }

    /// <summary>Performs the ExportReport operation.</summary>
    /// <param name="path">The path argument.</param>
    /// <param name="prescription">The prescription argument.</param>
    public void ExportReport(string path, Prescription prescription)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(prescription);
        using IDisposable? scope = AppEvents.BeginPrescription(_log, prescription.Id);
        try
        {
            using StreamWriter writer = new(path, append: false, new UTF8Encoding(false));
            writer.WriteLine(FormattableString.Invariant($"Рецепт № {prescription.Id}"));
            writer.WriteLine(_service.TotalOf(prescription)
                .ToString("F2", CultureInfo.InvariantCulture));
        }
        catch (IOException exception)
        {
            AppEvents.Error(_log, exception, "ExportReport");
            throw;
        }
    }
}
