using PharmacyStock.Core.Domain;
using PharmacyStock.Core.Errors;
namespace PharmacyStock.Core.Application;
/// <summary>Підмінне правило для навчальної рецептурності й лімітів відпуску.</summary>
public interface IDispensingPolicy
{
    /// <summary>Перевіряє допустимість відпуску до будь-якого списання.</summary>
    /// <param name="medicine">Перевірений препарат каталогу.</param>
    /// <param name="input">Структурно допустимий запит.</param>
    /// <param name="prescription">Знайдений рецепт або null.</param>
    /// <param name="today">Дата зміни, передана годинником.</param>
    void Check(Medicine medicine, DispenseInput input, Prescription? prescription, DateOnly today);
}
/// <summary>Правила R04–R05: рецепт, граничний вік і ліміт одиниць.</summary>
public sealed class DispensingPolicy : IDispensingPolicy
{
    /// <summary>Останній допустимий вік рецепта включно.</summary>
    public const int PrescriptionDays = 30;
    /// <inheritdoc/>
    /// <exception cref="DomainRuleException">Відпуск порушує правило рецепта чи кількості.</exception>
    public void Check(Medicine medicine, DispenseInput input, Prescription? prescription, DateOnly today)
    {
        if (input.Quantity > medicine.Rules.MaximumUnits)
        {
            throw new DomainRuleException("Перевищено навчальний ліміт одиниць.");
        }
        if (prescription is null)
        {
            if (medicine.Rules.RequiresPrescription || input.PrescriptionId.HasValue)
            {
                throw new DomainRuleException("Потрібен наявний підтверджений рецепт.");
            }
            return;
        }
        CheckPrescription(prescription, input, today);
    }
    private static void CheckPrescription(Prescription prescription, DispenseInput input, DateOnly today)
    {
        if (prescription.Status != PrescriptionStatus.Approved)
        {
            throw new DomainRuleException("Відпуск дозволено лише зі стану Approved.");
        }
        int age = today.DayNumber - prescription.CreatedAt.DayNumber;
        if (age is < 0 or > PrescriptionDays)
        {
            throw new DomainRuleException("Рецепт поза строком дії 0..30 днів.");
        }
        PrescriptionLine? line = prescription.Lines.SingleOrDefault(x => x.MedicineCode == input.Code);
        if (line is null || input.Quantity > line.Quantity)
        {
            throw new DomainRuleException("Позиція або кількість не відповідає рецепту.");
        }
    }
}
