using System.Globalization;
using System.Text;
using PharmacyStock.Core.Abstractions;
using PharmacyStock.Core.Domain;
using PharmacyStock.Core.Pricing;

namespace PharmacyStock.Core.Reporting;

/// <summary>Represents the PrescriptionCsvReport type.</summary>
public sealed class PrescriptionCsvReport
{
    private readonly IPricingPolicy _pricing;

    /// <summary>Initializes a new instance of the <see cref="PrescriptionCsvReport"/> class.</summary>
    /// <param name="pricing">The pricing argument.</param>
    public PrescriptionCsvReport(IPricingPolicy pricing)
    {
        ArgumentNullException.ThrowIfNull(pricing);
        _pricing = pricing;
    }

    /// <summary>Performs the BuildCsv operation.</summary>
    /// <param name="prescriptions">The prescriptions argument.</param>
    /// <returns>The result of the operation.</returns>
    public string BuildCsv(IReadOnlyList<Prescription> prescriptions)
    {
        ArgumentNullException.ThrowIfNull(prescriptions);
        StringBuilder text = new("id;total;status\n");
        foreach (Prescription prescription in prescriptions)
        {
            ArgumentNullException.ThrowIfNull(prescription);
            decimal total = PricingTotals.TotalOf(prescription.Lines, _pricing);
            text.AppendFormat(
                CultureInfo.InvariantCulture,
                "{0};{1:F2};{2}\n",
                prescription.Id,
                total,
                prescription.Status);
        }

        return text.ToString();
    }
}
