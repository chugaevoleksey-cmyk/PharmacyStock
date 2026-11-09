using Microsoft.Extensions.Logging;
using PharmacyStock.Core.Abstractions;
using PharmacyStock.Core.Domain;
using PharmacyStock.Core.Errors;
using PharmacyStock.Core.Logging;
using PharmacyStock.Core.Pricing;

namespace PharmacyStock.Core.Services;

/// <summary>Represents the PrescriptionService type.</summary>
public sealed class PrescriptionService
{
    private readonly IPrescriptionRepository _repository;
    private readonly IPricingPolicy _pricing;
    private readonly INotifier _notifier;
    private readonly ILogger<PrescriptionService> _log;

    /// <summary>Initializes a new instance of the <see cref="PrescriptionService"/> class.</summary>
    /// <param name="repository">The repository argument.</param>
    /// <param name="pricing">The pricing argument.</param>
    /// <param name="notifier">The notifier argument.</param>
    /// <param name="log">The log argument.</param>
    public PrescriptionService(
        IPrescriptionRepository repository,
        IPricingPolicy pricing,
        INotifier notifier,
        ILogger<PrescriptionService> log)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(pricing);
        ArgumentNullException.ThrowIfNull(notifier);
        ArgumentNullException.ThrowIfNull(log);
        _repository = repository;
        _pricing = pricing;
        _notifier = notifier;
        _log = log;
    }

    /// <summary>Performs the Place operation.</summary>
    /// <param name="prescription">The prescription argument.</param>
    public void Place(Prescription prescription)
    {
        ArgumentNullException.ThrowIfNull(prescription);
        using IDisposable? scope = AppEvents.BeginPrescription(_log, prescription.Id);
        decimal total = TotalOf(prescription);
        _repository.Add(prescription);
        _notifier.Notify(prescription.Id, total);
        AppEvents.Completed(_log, "Place", total);
    }

    /// <summary>Performs the TotalOf operation.</summary>
    /// <param name="prescription">The prescription argument.</param>
    /// <returns>The result of the operation.</returns>
    public decimal TotalOf(Prescription prescription)
    {
        ArgumentNullException.ThrowIfNull(prescription);
        return PricingTotals.TotalOf(prescription.Lines, _pricing);
    }

    /// <summary>Performs the AddLine operation.</summary>
    /// <param name="prescriptionId">The prescriptionId argument.</param>
    /// <param name="line">The line argument.</param>
    public void AddLine(int prescriptionId, PrescriptionLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        using IDisposable? scope = AppEvents.BeginPrescription(_log, prescriptionId);
        Prescription prescription = RequirePrescription(prescriptionId);
        if (prescription.Status != PrescriptionStatus.New)
        {
            throw new DomainRuleException(
                "prescription.editable", "Редагувати можна лише новий рецепт.", prescriptionId);
        }

        prescription.AddLine(line);
        Save(prescription);
        AppEvents.LineAdded(_log, line.Quantity);
    }

    /// <summary>Performs the Load operation.</summary>
    /// <param name="prescriptionId">The prescriptionId argument.</param>
    /// <returns>The result of the operation.</returns>
    public Prescription Load(int prescriptionId)
    {
        using IDisposable? scope = AppEvents.BeginPrescription(_log, prescriptionId);
        return RequirePrescription(prescriptionId);
    }

    /// <summary>Performs the Pay operation.</summary>
    /// <param name="prescriptionId">The prescriptionId argument.</param>
    /// <param name="amount">The amount argument.</param>
    public void Pay(int prescriptionId, decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(amount, 0m);
        using IDisposable? scope = AppEvents.BeginPrescription(_log, prescriptionId);
        AppEvents.Started(_log, "Pay", amount);
        Prescription prescription = RequirePrescription(prescriptionId);
        EnsurePayable(prescription);
        decimal due = TotalOf(prescription);
        if (amount < due)
        {
            throw new DomainRuleException(
                "prescription.amount", "Недостатня сума оплати.", prescriptionId);
        }

        if (amount > due)
        {
            AppEvents.Overpayment(_log, amount - due);
        }

        prescription.Approve();
        Save(prescription);
        AppEvents.Completed(_log, "Pay", amount);
    }

    /// <summary>Performs the Cancel operation.</summary>
    /// <param name="prescriptionId">The prescriptionId argument.</param>
    /// <param name="reason">The reason argument.</param>
    public void Cancel(int prescriptionId, string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        using IDisposable? scope = AppEvents.BeginPrescription(_log, prescriptionId);
        AppEvents.Started(_log, "Cancel", 0m);
        Prescription prescription = RequirePrescription(prescriptionId);
        if (prescription.Status is not PrescriptionStatus.New and not PrescriptionStatus.Approved)
        {
            throw new DomainRuleException(
                "prescription.cancellable", "Рецепт уже закрито.", prescriptionId);
        }

        try
        {
            prescription.Cancel();
            _repository.Save(prescription);
        }
        catch (IOException exception)
        {
            AppEvents.Error(_log, exception, "Cancel");
            throw;
        }

        AppEvents.Completed(_log, "Cancel", 0m);
    }

    private static void EnsurePayable(Prescription prescription)
    {
        if (prescription.Status != PrescriptionStatus.New)
        {
            throw new DomainRuleException(
                "prescription.payable", "Стан рецепта не дозволяє оплату.", prescription.Id);
        }

        if (prescription.Lines.Count == 0)
        {
            throw new DomainRuleException(
                "prescription.lines", "Рецепт не містить позицій.", prescription.Id);
        }
    }

    private Prescription RequirePrescription(int prescriptionId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(prescriptionId);
        try
        {
            return _repository.GetById(prescriptionId)
                ?? throw new DomainRuleException(
                    "prescription.exists", "Рецепт не знайдено.", prescriptionId);
        }
        catch (IOException exception)
        {
            AppEvents.Error(_log, exception, "Load");
            throw;
        }
    }

    private void Save(Prescription prescription)
    {
        try
        {
            _repository.Save(prescription);
        }
        catch (IOException exception)
        {
            AppEvents.Error(_log, exception, "Save");
            throw;
        }
    }
}
