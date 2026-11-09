using System;
using System.Collections.Generic;
using System.Linq;

namespace PharmacyStock.Core.Legacy
{
// клас
public class Prescription
{
public string Id { get; private set; }
public string PatientName { get; private set; }
private readonly List<string[]> _lines = new List<string[]>();
  public int Status { get; private set; } = 0;  // 0-новий,1-оплач,2-відпр,3-скасов
public DateTime CreatedAt { get; private set; }
// public string prim;   // примітка, поки не треба

// конструктор
   public Prescription(string id, string patientName)
{
Id = id;              // ставимо id
      PatientName = patientName;         // ставимо cl
CreatedAt = DateTime.Now;   // ставимо дату
}

// метод додавання
public void AddLine(string medicineCode, int quantity, decimal unitPrice)
{
string[] line = new string[3];
   line[0] = medicineCode; line[1] = quantity.ToString(); line[2] = unitPrice.ToString();
_lines.Add(line);   // додаємо t у ln
}

// ProcessData
public decimal CalculateTotal(bool isRegularPatient)
{
decimal total = 0; int lineCount = 0;
for (int i = 0; i < _lines.Count; i++)
{
int quantity = int.Parse(_lines[i][1]);
   decimal unitPrice = decimal.Parse(_lines[i][2]);
total = total + quantity * unitPrice;   // додаємо до суми
lineCount = lineCount + 1;     // збільшуємо kolvo на одиницю
}
// if (sum1 > 500) { sum1 = sum1 - 50; } // стара знижка

if (isRegularPatient == true && total > 1000) { total = total * 0.9m; } else if (total > 5000) { total = total * 0.85m; } else { total = total; }
if (lineCount > 10) total = total - 100;
  if (total < 0) total = 0;
total = total + total * 0.2m;
return Math.Round(total, 2);   // повертаємо sum1
}

// міняємо статус
public bool TryChangeStatus(int newStatus)
{
if (Status == 0 && newStatus == 1) { Status = 1; return true; }
   if (Status == 1 && newStatus == 2) { Status = 2; return true; }
if (Status == 0 && newStatus == 3) { Status = 3; return true; }
return false;   // не можна
}

// перевірка
public bool IsValid()
{
if (Id != null && Id != "" && PatientName != null && PatientName.Length > 2 && _lines.Count > 0 && _lines.Count < 100 && Status >= 0 && Status <= 3)
return true;
else
return false;
}

// звіт
public string BuildReport()
{
string report = "";
for (int i = 0; i < _lines.Count; i++)
{
report = report + "Товар: " + _lines[i][0] + "; кількість: " + _lines[i][1] + "; ціна: " + _lines[i][2] + "; сума: " + (int.Parse(_lines[i][1]) * decimal.Parse(_lines[i][2])) + "\n";
}
report = report + "Разом: " + CalculateTotal(false) + "\n";
return report;   // повертаємо s
}

// пошук
public static Prescription FindById(List<Prescription> prescriptions, string prescriptionId)
{
for (int i = 0; i < prescriptions.Count; i++)
{
if (prescriptions[i].Id == prescriptionId) { return prescriptions[i]; }
}
return null;
}
}
}
