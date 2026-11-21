using PharmacyStock.Core.Domain;
using PharmacyStock.Core.Errors;
using Xunit;
namespace PharmacyStock.Tests;

public sealed class PrescriptionTests
{
    [Fact]
    public void Approve_Empty_Rejects()
    {
        var prescription = new Prescription(1, 1, new DateOnly(2026, 11, 21));
        Assert.Throws<DomainRuleException>(prescription.Approve);
    }
}
