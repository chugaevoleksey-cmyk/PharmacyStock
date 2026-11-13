namespace PharmacyStock.Legacy;
public static class LegacyReportBuilder
{
    public static string Build(int docId, string clientName, List<LegacyPrescriptionLine> items,
        string currency, decimal discount, decimal ship, decimal total)
    {
        string txt = "Документ #" + docId + "\n";
        txt += "Клієнт: " + clientName + "\n";
        for (int i = 0; i < items.Count; i++)
        {
            txt += items[i].Code + " x" + items[i].Qty + " = " +
                (items[i].Qty * items[i].Price)
                    .ToString("0.00") + " " + currency + "\n";
        }
        txt += "Знижка: " + discount.ToString("0.00") +
            " " + currency + "\n";
        txt += "Доставка: " + ship.ToString("0.00") +
            " " + currency + "\n";
        txt += "Разом: " + total.ToString("0.00") +
            " " + currency + "\n";

        return txt;
    }
}
