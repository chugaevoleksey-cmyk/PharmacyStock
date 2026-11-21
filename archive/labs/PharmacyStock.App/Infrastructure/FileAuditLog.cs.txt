using System.Globalization;
using System.Text;

namespace PharmacyStock.App.Infrastructure;

/// <summary>Represents the FileAuditLog type.</summary>
public sealed class FileAuditLog : IDisposable
{
    private readonly StreamWriter _writer;
    private bool _disposed;

    /// <summary>Initializes a new instance of the <see cref="FileAuditLog"/> class.</summary>
    /// <param name="path">The path argument.</param>
    public FileAuditLog(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        _writer = new StreamWriter(path, append: true, new UTF8Encoding(false));
    }

    /// <summary>Performs the Write operation.</summary>
    /// <param name="operation">The operation argument.</param>
    /// <param name="prescriptionId">The prescriptionId argument.</param>
    public void Write(string operation, int prescriptionId)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(operation);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(prescriptionId);
        _writer.WriteLine(string.Format(
            CultureInfo.InvariantCulture,
            "{0:O};{1};{2}",
            DateTimeOffset.UtcNow,
            operation,
            prescriptionId));
    }

    /// <summary>Performs the Dispose operation.</summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _writer.Dispose();
        GC.SuppressFinalize(this);
    }
}
