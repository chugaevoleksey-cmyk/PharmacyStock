using System;
using System.Collections.Generic;

namespace PharmacyStock.Legacy;

public class LegacyPatient
{
    public int Id;
    public string Name = "";
    public string? Email = "";
    public ClientKind Kind = ClientKind.Regular;
    public int DoneCount;
    public DateTime SinceUtc;
    public string Describe()
    {
        string s = Name.Trim().ToUpper();
        if (Kind == ClientKind.Vip) s += " [VIP]";
        if (DoneCount > PricingRules.LoyalOrders) s += " [ЛОЯЛЬНИЙ]";
        s += " <" + Email!.ToLower() + ">";
        int years = DateTime.Now.Year - SinceUtc.Year;
        s += " стаж " + years;
        return s;
    }
}

public class LegacyPrescriptionLine
{
    public string Code = "";
    public int Qty;
    public decimal Price;
    public string Group = "";
}

public class LegacyPrescriptionProcessor
{
    private readonly List<string> _log = new();

    public string Handle(PrescriptionRequest request)
    {
        if (request.Items == null) return "ERR: null";
        if (request.Items.Count == 0) return "ERR: empty";
        if (request.State != DocState.New && request.State != DocState.Paid) return "ERR: state";
        if (request.Patient.Email == null || !request.Patient.Email.Contains("@")) return "ERR: mail";
        _log.Add("ok " + request.DocId);

        decimal subtotal = LineTotals.Subtotal(request.Items);
        decimal discount = LegacyDiscountPolicy.For(subtotal, request.Patient.Kind, request.Patient.DoneCount);
        decimal ship = LegacyShippingPolicy.For(subtotal - discount);
        decimal total = subtotal - discount + ship;

        string txt = LegacyReportBuilder.Build(request, discount, ship, total);

        if (request.SendMail)
        {
            _log.Add("mail -> " + request.Patient.Email);
        }
        return txt;
    }

    public decimal Preview(ClientKind clientKind, int clientDone,
        List<LegacyPrescriptionLine> items)
    {
        decimal sum = LineTotals.Subtotal(items);
        decimal discount = LegacyDiscountPolicy.For(sum, clientKind, clientDone);
        return sum - discount + LegacyShippingPolicy.For(sum - discount);
    }



    public string DumpLog()
    {
        string r = "";
        foreach (var l in _log) r += l + "\n";
        return r;
    }




}
