using System.Globalization;
using System.Text;
using PharmacyStock.Core.Legacy;

// Однакова локаль для відтворюваного порівняння до і після.
CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("uk-UA");
Console.OutputEncoding = new UTF8Encoding(false);

var z = new zakaz("A-1001", "Іваненко");
z.Add("SKU-1", 3, 250m);
z.Add("SKU-2", 12, 90m);

Console.WriteLine(z.ProcessData(false));
Console.WriteLine(z.ProcessData(true));
Console.WriteLine(z.Ck());
Console.WriteLine(z.Chg(1));
Console.Write(z.Rep());
