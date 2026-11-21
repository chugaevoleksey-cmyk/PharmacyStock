using PharmacyStock.Core.Domain;
using Xunit;
namespace PharmacyStock.Tests;

public sealed class QueryTests
{
    [Theory]
    [InlineData("code", "rx", 1)]
    [InlineData("name", "ЛІКИ", 2)]
    [InlineData("code", "absent", 0)]
    public void Search_TwoCriteria_ReturnsExpected(string criterion, string text, int expected)
    {
        var scenario = new Scenario();
        Assert.Equal(expected, scenario.Service.Search(criterion, text).Count);
    }
    [Fact]
    public void Expiring_Empty_ReturnsEmpty()
    {
        Assert.Empty(new Scenario().Service.Expiring(30));
    }
    [Fact]
    public void Expiring_Boundaries_SortsAndExcludesExpiredZeroAndOutside()
    {
        var scenario = new Scenario();
        scenario.Batch(1, 30);
        scenario.Batch(1, -1);
        scenario.Batch(0, 1);
        scenario.Batch(1, 31);
        scenario.Batch(1, 0);
        int[] expected = { 5, 1 };
        Assert.Equal(expected, scenario.Service.Expiring(30).Select(x => x.Id));
        Assert.Single(scenario.Service.Expiring(0));
    }
    [Theory]
    [InlineData(-1)]
    [InlineData(91)]
    public void Expiring_InvalidHorizon_Rejects(int days)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Scenario().Service.Expiring(days));
    }
    [Fact]
    public void Core_References_ExcludeInfrastructure()
    {
        var references = typeof(Prescription).Assembly.GetReferencedAssemblies();
        Assert.DoesNotContain(references, x => x.Name!.Contains("Infrastructure", StringComparison.Ordinal));
    }
}
