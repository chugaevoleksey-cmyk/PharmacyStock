using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using PharmacyStock.App.Infrastructure;
using PharmacyStock.App.Input;
using PharmacyStock.Core.Abstractions;
using PharmacyStock.Core.Domain;
using PharmacyStock.Core.Errors;
using PharmacyStock.Core.Notifications;
using PharmacyStock.Core.Pricing;
using PharmacyStock.Core.Services;
using PharmacyStock.Core.Storage;
using Xunit;

namespace PharmacyStock.Tests;

public sealed class ErrorHandlingTests
{
    [Theory]
    [InlineData("дві")]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("2147483648")]
    [InlineData("")]
    public void ParserRejectsInvalidQuantitiesWithoutExceptions(string quantity)
    {
        var result = PrescriptionLineParser.Parse("MED-1", quantity, "10.50");
        Assert.False(result.IsSuccess);
        Assert.Contains("Кількість", result.Error);
    }

    [Theory]
    [InlineData("-1")]
    [InlineData("10,50")]
    [InlineData("abc")]
    [InlineData("")]
    public void ParserRejectsInvalidPricesWithoutExceptions(string price)
    {
        Assert.False(PrescriptionLineParser.Parse("MED-1", "2", price).IsSuccess);
    }

    [Fact]
    public void ParserUsesInvariantDecimalCulture()
    {
        CultureInfo previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("uk-UA");
            var result = PrescriptionLineParser.Parse(" MED-1 ", "2", "10.50");
            Assert.True(result.IsSuccess);
            Assert.Equal("MED-1", result.Value.MedicineCode);
            Assert.Equal(21m, result.Value.Amount);
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void ParserRejectsEmptyCodeAndAmountOverflow()
    {
        Assert.False(PrescriptionLineParser.Parse("  ", "2", "10").IsSuccess);
        string maximum = decimal.MaxValue.ToString(CultureInfo.InvariantCulture);
        Assert.False(PrescriptionLineParser.Parse("MED-1", "2", maximum).IsSuccess);
    }

    [Fact]
    public void ResultFailureAndDefaultCannotBeReadAsSuccess()
    {
        Result<string> failure = Result<string>.Fail("bad input");
        Assert.Throws<InvalidOperationException>(() => failure.Value);
        Result<string> uninitialized = default;
        Assert.False(uninitialized.IsSuccess);
        Assert.NotEmpty(uninitialized.Error);
        Assert.Throws<InvalidOperationException>(() => uninitialized.Value);
        Assert.Throws<ArgumentNullException>(() => Result<string>.Ok(null!));
    }

    [Fact]
    public void DomainExceptionRetainsRuleContextAndInnerException()
    {
        IOException cause = new("storage failed");
        DomainRuleException wrapped = new("prescription.store", "Не збережено.", cause);
        Assert.Equal("prescription.store", wrapped.Rule);
        Assert.Same(cause, wrapped.InnerException);
        DomainRuleException contextual = new("prescription.exists", "Не знайдено.", 19);
        Assert.Equal(19, contextual.PrescriptionId);
    }

    [Fact]
    public void MissingPrescriptionRaisesNamedRuleInsteadOfNull()
    {
        PrescriptionService service = Service(new InMemoryPrescriptionRepository());
        var error = Assert.Throws<DomainRuleException>(() => service.Load(19));
        Assert.Equal("prescription.exists", error.Rule);
        Assert.Equal(19, error.PrescriptionId);
    }

    [Fact]
    public void UnderpaymentDoesNotChangeThePrescriptionState()
    {
        InMemoryPrescriptionRepository repository = new();
        Prescription prescription = PrescriptionWithLine();
        repository.Add(prescription);
        PrescriptionService service = Service(repository);
        var error = Assert.Throws<DomainRuleException>(() => service.Pay(1, 1m));
        Assert.Equal("prescription.amount", error.Rule);
        Assert.Equal(PrescriptionStatus.New, prescription.Status);
    }

    [Fact]
    public void TypedLineCannotBeAddedAfterApproval()
    {
        InMemoryPrescriptionRepository repository = new();
        Prescription prescription = PrescriptionWithLine();
        repository.Add(prescription);
        prescription.Approve();
        var error = Assert.Throws<DomainRuleException>(() =>
            Service(repository).AddLine(1, new PrescriptionLine("MED-2", 1, 1m)));
        Assert.Equal("prescription.editable", error.Rule);
        Assert.Single(prescription.Lines);
    }

    [Fact]
    public void StructuredLoggingKeepsNamedValuesAndDocumentScope()
    {
        InMemoryPrescriptionRepository repository = new();
        repository.Add(PrescriptionWithLine());
        RecordingLogger<PrescriptionService> logger = new();
        Service(repository, logger: logger).Pay(1, 200m);
        Assert.Contains(logger.Entries, e => e.Level == LogLevel.Debug);
        Assert.Contains(logger.Entries, e => e.Level == LogLevel.Information);
        LogEntry warning = Assert.Single(logger.Entries, e => e.Level == LogLevel.Warning);
        Assert.Equal(10m, warning.Values["Extra"]);
        Assert.Equal(1, warning.Scope["PrescriptionId"]);
        Assert.Equal("Переплата {Extra}", warning.Values["{OriginalFormat}"]);
    }

    [Fact]
    public void CancelLogsIOExceptionAndPreservesItsOriginalStack()
    {
        FailingRepository repository = new(PrescriptionWithLine());
        RecordingLogger<PrescriptionService> logger = new();
        IOException error = Assert.Throws<IOException>(() =>
            Service(repository, logger: logger).Cancel(1, "Навчальний сценарій"));
        Assert.Contains(nameof(FailingRepository.ThrowStorageFailure), error.StackTrace);
        LogEntry entry = Assert.Single(logger.Entries, e => e.Level == LogLevel.Error);
        Assert.Same(error, entry.Exception);
        Assert.Equal(1, entry.Scope["PrescriptionId"]);
        Assert.Equal("Cancel", entry.Values["Operation"]);
    }

    [Fact]
    public void ConsoleRetriesBadInputThenAddsTypedLine()
    {
        using StringReader input = new("MED-1\nдві\n10\nMED-1\n2\n10.50\n");
        using StringWriter output = new(CultureInfo.InvariantCulture);
        InMemoryPrescriptionRepository repository = new();
        repository.Add(new Prescription(1, 19, new DateOnly(2026, 3, 10)));
        PrescriptionConsole session = new(input, output);
        Assert.True(session.ReadAndAddLine(Service(repository), 1));
        Assert.Contains("Помилка вводу: Кількість", output.ToString());
        Assert.Equal(21m, Assert.Single(repository.GetById(1)!.Lines).Amount);
    }

    [Fact]
    public void TryParseCountsOnlyPositiveIntegers()
    {
        string?[] rows = { "дві", "2", "0", "1x", "2147483648", "-1", null, "4" };
        Assert.Equal(2, QuantityInput.CountValidQuantities(rows));
    }

    [Fact]
    public void AuditFileClosesOnExceptionAndDisposeIsIdempotent()
    {
        using TemporaryDirectory directory = new();
        string path = Path.Combine(directory.Path, "audit.log");
        FileAuditLog? audit = null;
        Action failingOperation = () =>
        {
            using (audit = new FileAuditLog(path))
            {
                audit.Write("probe", 1);
                throw new InvalidOperationException("Controlled test failure");
            }
        };
        Assert.Throws<InvalidOperationException>(failingOperation);
        using (FileStream exclusive = new(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
        {
            Assert.True(exclusive.Length > 0);
        }

        audit!.Dispose();
        Assert.Throws<ObjectDisposedException>(() => audit.Write("late", 1));
        Assert.Contains(";probe;1", File.ReadAllText(path));
    }

    [Fact]
    public void ExportClosesItsWriterWhenPriceCalculationThrows()
    {
        using TemporaryDirectory directory = new();
        string path = Path.Combine(directory.Path, "report.txt");
        PrescriptionService service = Service(new InMemoryPrescriptionRepository(),
            new ThrowingPricingPolicy());
        ReportExporter exporter = new(service, NullLogger<ReportExporter>.Instance);
        Assert.Throws<InvalidOperationException>(() =>
            exporter.ExportReport(path, PrescriptionWithLine()));
        using FileStream exclusive = new(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        Assert.True(exclusive.Length > 0);
    }

    [Fact]
    public void MissingExportDirectoryIsLoggedWithTheExceptionObject()
    {
        using TemporaryDirectory directory = new();
        RecordingLogger<ReportExporter> logger = new();
        ReportExporter exporter = new(Service(new InMemoryPrescriptionRepository()), logger);
        Assert.Throws<DirectoryNotFoundException>(() => exporter.ExportReport(
            Path.Combine(directory.Path, "missing", "report.txt"), PrescriptionWithLine()));
        LogEntry error = Assert.Single(logger.Entries);
        Assert.IsType<DirectoryNotFoundException>(error.Exception);
        Assert.Equal(LogLevel.Error, error.Level);
        Assert.Equal(1, error.Scope["PrescriptionId"]);
    }

    private static PrescriptionService Service(
        IPrescriptionRepository repository, IPricingPolicy? pricing = null,
        ILogger<PrescriptionService>? logger = null)
    {
        return new PrescriptionService(repository, pricing ?? new DiscountPricingPolicy(0.05m),
            new TextWriterNotifier(TextWriter.Null), logger ?? NullLogger<PrescriptionService>.Instance);
    }

    private static Prescription PrescriptionWithLine()
    {
        Prescription prescription = new(1, 19, new DateOnly(2026, 3, 10));
        prescription.AddLine(new PrescriptionLine("MED-1", 2, 100m));
        return prescription;
    }

    private sealed class ThrowingPricingPolicy : IPricingPolicy
    {
        public decimal PriceOf(PrescriptionLine line)
        {
            throw new InvalidOperationException("Controlled calculation failure");
        }
    }

    private sealed class FailingRepository(Prescription prescription) : IPrescriptionRepository
    {
        public void Add(Prescription item) => throw new NotSupportedException();

        public Prescription? GetById(int id) => prescription;

        public IReadOnlyList<Prescription> GetAll() => new[] { prescription };

        public void Save(Prescription item) => ThrowStorageFailure();

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowStorageFailure() => throw new IOException("Controlled storage failure");
    }

    private sealed class TemporaryDirectory : IDisposable
    {
        public TemporaryDirectory()
        {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(),
                "PharmacyStockLab5-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path);
        }

        public string Path { get; }

        public void Dispose()
        {
            foreach (string file in Directory.EnumerateFiles(Path))
            {
                File.Delete(file);
            }

            Directory.Delete(Path);
        }
    }

    private sealed record LogEntry(
        LogLevel Level, Exception? Exception, Dictionary<string, object?> Values,
        Dictionary<string, object?> Scope);

    private sealed class RecordingLogger<T> : ILogger<T>
    {
        private readonly Stack<Dictionary<string, object?>> _scopes = new();

        public List<LogEntry> Entries { get; } = new();

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            _scopes.Push(ValuesOf(state));
            return new Scope(() => _scopes.Pop());
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
            Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Dictionary<string, object?> context = new();
            foreach (var scope in _scopes.Reverse())
            {
                foreach (var pair in scope)
                {
                    context[pair.Key] = pair.Value;
                }
            }

            Entries.Add(new LogEntry(logLevel, exception, ValuesOf(state), context));
        }

        private static Dictionary<string, object?> ValuesOf<TState>(TState state)
        {
            return state is IEnumerable<KeyValuePair<string, object?>> values
                ? values.ToDictionary(x => x.Key, x => x.Value)
                : new Dictionary<string, object?>();
        }

        private sealed class Scope(Action dispose) : IDisposable
        {
            public void Dispose() => dispose();
        }
    }
}
