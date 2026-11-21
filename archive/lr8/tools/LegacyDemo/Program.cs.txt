using System.Globalization;
using PharmacyStock.Legacy;
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
var patient = new LegacyPatient { Id = 19, Name = "Іван", Email = "a@b.c", Kind = ClientKind.Vip, DoneCount = 3, SinceUtc = new DateTime(2020, 1, 1) };
var items = new List<LegacyPrescriptionLine>
{
    new() { Code = "A-1", Qty = 2, Price = 150m },
    new() { Code = "B-2", Qty = 1, Price = 400m }
};
var request = new PrescriptionRequest(1001, patient, items, DocState.New, "UAH", new DateTime(2026, 3, 1), true);
var processor = new LegacyPrescriptionProcessor();
Console.WriteLine("ЛР6 — результат після рефакторингу");
Console.Write(processor.Handle(request));
Console.WriteLine("Помилки в початковому порядку:");
Console.WriteLine(processor.Handle(request with { Items = null }));
Console.WriteLine(processor.Handle(request with { Items = new() }));
Console.WriteLine(processor.Handle(request with { State = DocState.Closed }));
var invalidPatient = new LegacyPatient { Name = "Іван", Email = "no-mail" };
Console.WriteLine(processor.Handle(request with { Patient = invalidPatient }));
Console.WriteLine("Межа доставки: " + processor.Preview(ClientKind.Regular, 0, new() { new() { Qty = 1, Price = 1000m } }));
Console.WriteLine("Журнал (поштова дія лише записується):");
Console.Write(processor.DumpLog());
