namespace PharmacyStock.Core.Lab03Legacy;
public sealed class PrescriptionLine
{
    public string MedicineCode { get; set; } = "";
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
