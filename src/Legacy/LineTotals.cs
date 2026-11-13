namespace PharmacyStock.Legacy;
public static class LineTotals
{
    public static decimal Subtotal(List<LegacyPrescriptionLine> items)
    {
        decimal sum = 0m;
        foreach (var item in items) sum += item.Qty * item.Price;
        return sum;
    }
}
