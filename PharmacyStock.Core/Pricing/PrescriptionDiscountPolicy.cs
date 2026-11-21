using PharmacyStock.Core.Abstractions;

namespace PharmacyStock.Core.Pricing;

/// <summary>Calculates the promotional discount on the whole prescription amount.</summary>
public sealed class PrescriptionDiscountPolicy : IPrescriptionDiscountPolicy
{
    /// <summary>The additional rate for a regular patient.</summary>
    public const decimal RegularRate = 0.05m;

    /// <summary>The additional rate for a large prescription.</summary>
    public const decimal BigPrescriptionRate = 0.10m;

    /// <summary>The minimum amount qualifying for the large-prescription rate.</summary>
    public const decimal BigPrescriptionFrom = 1000m;

    /// <summary>The maximum promotional discount.</summary>
    public const decimal MaxDiscount = 150m;

    /// <summary>Calculates a discount independently from individual-line pricing policies.</summary>
    /// <param name="amount">Nonnegative prescription amount before the promotion.</param>
    /// <param name="isRegular">Whether the patient qualifies for the regular-patient rate.</param>
    /// <returns>The discount rounded to two decimal places using ToEven.</returns>
    public decimal DiscountFor(decimal amount, bool isRegular)
    {
        if (amount < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Сума не може бути від’ємною.");
        }

        decimal rate = 0m;
        if (isRegular)
        {
            rate += RegularRate;
        }

        if (amount >= BigPrescriptionFrom)
        {
            rate += BigPrescriptionRate;
        }

        decimal discount = decimal.Round(amount * rate, 2);
        if (discount > MaxDiscount)
        {
            discount = MaxDiscount;
        }

        return discount;
    }
}
