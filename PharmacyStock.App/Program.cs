using System.Globalization;
using System.Text;
using PharmacyStock.Core.Legacy;

// Однакова локаль для відтворюваного порівняння до і після.
CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("uk-UA");
Console.OutputEncoding = new UTF8Encoding(false);

var prescription = new Prescription("A-1001", "Іваненко");
prescription.AddLine("SKU-1", 3, 250m);
prescription.AddLine("SKU-2", 12, 90m);

Console.WriteLine(prescription.CalculateTotal(false));
Console.WriteLine(prescription.CalculateTotal(true));
Console.WriteLine(prescription.IsValid());
Console.WriteLine(prescription.TryChangeStatus(1));
Console.Write(prescription.BuildReport());
