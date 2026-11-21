using PharmacyStock.Core.Domain;
using Xunit;
namespace PharmacyStock.Tests;
public sealed class ArchitectureTests
{
    [Fact]
    public void Core_AssemblyReferences_ExcludeOuterLayers()
    {
        string[] forbidden = { "System.Console", "System.Text.Json", "System.IO.FileSystem", "PharmacyStock.Infrastructure", "PharmacyStock.App" };
        string[] references = typeof(Prescription).Assembly.GetReferencedAssemblies().Select(x => x.Name!).ToArray();
        Assert.Empty(references.Intersect(forbidden, StringComparer.Ordinal));
    }
    [Fact]
    public void Receive_InvalidInput_LogsWarningWithoutRawInput()
    {
        var scenario = new Scenario();
        scenario.Service.Receive(new Core.Application.ReceiptInput(1, "private-token", 1, scenario.Clock.Today));
        string entry = Assert.Single(scenario.Log.Events);
        Assert.StartsWith("Warning|UC-02|", entry, StringComparison.Ordinal);
        Assert.DoesNotContain("private-token", entry, StringComparison.Ordinal);
    }
}
