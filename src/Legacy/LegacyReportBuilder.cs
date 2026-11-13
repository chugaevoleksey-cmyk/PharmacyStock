namespace PharmacyStock.Legacy;
public static class LegacyReportBuilder
{
    public static string Build(PrescriptionRequest request, decimal discount, decimal ship, decimal total)
    {
        string txt = "Документ #" + request.DocId + "\n";
        txt += "Клієнт: " + request.Patient.Name + "\n";
        for (int i = 0; i < request.Items!.Count; i++)
        {
            txt += request.Items![i].Code + " x" + request.Items![i].Qty + " = " +
                (request.Items![i].Qty * request.Items![i].Price)
                    .ToString("0.00") + " " + request.Currency + "\n";
        }
        txt += "Знижка: " + discount.ToString("0.00") +
            " " + request.Currency + "\n";
        txt += "Доставка: " + ship.ToString("0.00") +
            " " + request.Currency + "\n";
        txt += "Разом: " + total.ToString("0.00") +
            " " + request.Currency + "\n";

        return txt;
    }
}
