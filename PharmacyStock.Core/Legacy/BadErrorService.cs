using PharmacyStock.Core.Abstractions;
using PharmacyStock.Core.Domain;
using PharmacyStock.Core.Pricing;

namespace PharmacyStock.Core.Legacy;

public sealed class BadErrorService
{
    private readonly IPrescriptionRepository _repo;
    private readonly IPricingPolicy _pricing;

    public BadErrorService(IPrescriptionRepository repo, IPricingPolicy pricing)
    {
        _repo = repo;
        _pricing = pricing;
    }

    public void AddLine(int id, string code, string quantity, string price)
    {
        var prescription = _repo.GetById(id);
        var count = int.Parse(quantity);
        var cost = decimal.Parse(price);
        prescription!.AddLine(new PrescriptionLine(code, count, cost));
        _repo.Save(prescription);
    }

    public int Pay(int id, decimal amount)
    {
        var prescription = _repo.GetById(id);
        if (prescription == null) return -1;
        if (prescription.Status != PrescriptionStatus.New) return -2;
        if (amount < TotalOf(prescription)) return -3;
        prescription.Approve();
        _repo.Save(prescription);
        return 0;
    }

    public Prescription? Load(int id)
    {
        try { return _repo.GetById(id); }
        catch (Exception) { }
        return null;
    }

    public void Cancel(int id, string reason)
    {
        try
        {
            var prescription = _repo.GetById(id);
            prescription!.Cancel();
            _repo.Save(prescription);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Щось пішло не так");
            throw ex;
        }
    }

    public int CountValidQuantities(string[] rows)
    {
        var valid = 0;
        foreach (var row in rows)
        {
            try
            {
                var quantity = int.Parse(row);
                if (quantity > 0) valid++;
            }
            catch (FormatException) { }
        }
        return valid;
    }

    public void ExportReport(string path, Prescription prescription)
    {
        var writer = new StreamWriter(path);
        writer.WriteLine(prescription.Id);
        writer.WriteLine(TotalOf(prescription));
        writer.Close();
    }

    private decimal TotalOf(Prescription prescription)
        => PricingTotals.TotalOf(prescription.Lines, _pricing);
}
