using System;
using System.Collections.Generic;

namespace PharmacyStock.Legacy;

public class LegacyPatient
{
    public int Id;
    public string Name = "";
    public string Email = "";
    public ClientKind Kind = ClientKind.Regular;
    public int DoneCount;
    public DateTime SinceUtc;
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
    private decimal _tmpSum;
    private decimal _tmpDiscount;
    private readonly List<string> _log = new();

    public string Handle(int docId, int clientId,
        string clientName, string? clientMail,
        ClientKind clientKind, int clientDone,
        List<LegacyPrescriptionLine>? items, DocState state,
        string currency, DateTime createdAt, bool sendMail)
    {
        if (items == null) return "ERR: null";
        if (items.Count == 0) return "ERR: empty";
        if (state != DocState.New && state != DocState.Paid) return "ERR: state";
        if (clientMail == null || !clientMail.Contains("@")) return "ERR: mail";
        _log.Add("ok " + docId);

        _tmpSum = LineTotals.Subtotal(items);
        _tmpDiscount = LegacyDiscountPolicy.For(_tmpSum, clientKind, clientDone);
        decimal ship = LegacyShippingPolicy.For(_tmpSum - _tmpDiscount);
        decimal total = _tmpSum - _tmpDiscount + ship;

        string txt = LegacyReportBuilder.Build(docId, clientName, items, currency, _tmpDiscount, ship, total);

        if (sendMail)
        {
            _log.Add("mail -> " + clientMail);
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

    public string DescribeClient(LegacyPatient c)
    {
        string s = c.Name.Trim().ToUpper();
        if (c.Kind == ClientKind.Vip) s += " [VIP]";
        if (c.DoneCount > PricingRules.LoyalOrders) s += " [ЛОЯЛЬНИЙ]";
        s += " <" + c.Email.ToLower() + ">";
        int years = DateTime.Now.Year - c.SinceUtc.Year;
        s += " стаж " + years;
        return s;
    }

    public string DumpLog()
    {
        string r = "";
        foreach (var l in _log) r += l + "\n";
        return r;
    }




}
