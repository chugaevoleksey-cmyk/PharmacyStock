using System.Globalization;
using Final = PharmacyStock.Core.Legacy.Prescription;
try
{
    int checks = 0;
    void Equal(object expected, object actual, string context)
    {
        checks++;
        if (!Equals(expected, actual))
            throw new Exception($"Mismatch in {context}: {expected} != {actual}");
    }
    foreach (string culture in new[] { "uk-UA", "en-US", "fr-FR", "" })
    {
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
        var random = new Random(19);
        foreach (int count in new[] { 0, 1, 2, 10, 11, 99, 100, 101 })
        {
            foreach (decimal price in new[] { -1m, 0m, 1m, 1000m, 1000.01m, 5000m, 5000.01m })
            {
                foreach (string name in new[] { null, "", "AB", "ABC", "Іваненко" })
                {
                    var old = new Baseline.zakaz("A-1001", name);
                    var updated = new Final("A-1001", name);
                    for (int i=0; i<count; i++)
                    {
                        int quantity = i==0 ? 1 : random.Next(-2, 16);
                        decimal unitPrice = i==0 ? price : random.Next(0, 50000) / 100m;
                        old.Add("MED-"+i,quantity,unitPrice);
                        updated.AddLine("MED-"+i,quantity,unitPrice);
                    }
                    foreach(bool regular in new[] { false,true })
                    {
                        Equal(old.ProcessData(regular), updated.CalculateTotal(regular), "total value");
                        Equal(old.ProcessData(regular).ToString(),
                            updated.CalculateTotal(regular).ToString(), "total text and decimal scale");
                    }
                    Equal(old.Ck(),updated.IsValid(),"validity");
                    Equal(old.Rep(),updated.BuildReport(),"exact report");
                    foreach(int status in new[] { -1,0,1,1,2,3,4 })
                    {
                        Equal(old.Chg(status),updated.TryChangeStatus(status),"status attempt");
                        Equal(old.st,updated.Status,"status state");
                    }
                    Equal(old.id,updated.Id,"identifier");
                    Equal(old.cl,updated.PatientName,"patient name");
                }
            }
        }
        var one = new Final("A","Name");
        Equal(one,Final.FindById(new List<Final>{one},"A"),"search match");
        Equal(null,Final.FindById(new List<Final>{one},"B"),"search miss");
        Equal(null,Final.FindById(new List<Final>(),"A"),"search empty");
    }
    Console.WriteLine($"PASS: {checks} comparisons, four cultures, thresholds, line counts, null names, statuses and search.");
    return 0;
}
catch(Exception error)
{
    Console.Error.WriteLine(error.Message);
    return 1;
}
