namespace PharmacyStock.Core.Documents;

/// <summary>Calculates prescription totals without external output.</summary>
public static class DocumentTotalCalculator
{
    /// <summary>Calculates a total after validating the complete request.</summary>
    /// <param name="request">Calculation request.</param>
    /// <returns>The calculated total and next state.</returns>
    public static TotalResult Calculate(TotalRequest request)
    {
        DocumentGuards.EnsureRequestValid(request);
        decimal total = SubtotalOf(request.Lines);
        total -= ClientDiscountOf(total, request.Patient.IsRegular);
        total -= VolumeDiscountOf(total);
        total -= CouponDiscountOf(total, request.CouponCode);
        total -= WeekendDiscountOf(total, request.CreatedAt);
        total += DeliveryFeeOf(total, request.DeliveryPrice);
        total = request.Status == DocumentStatus.Cancelled
            ? 0m
            : Math.Round(Math.Max(total, 0m), PricingRules.CurrencyDecimals);
        return new TotalResult { Total = total, Status = NextStatusOf(request.Status, total) };
    }

    /// <summary>Calculates the subtotal with the quantity discount.</summary>
    /// <param name="lines">Medicine lines.</param>
    /// <returns>The calculated amount.</returns>
    public static decimal SubtotalOf(IReadOnlyList<PrescriptionLine> lines)
    {
        DocumentGuards.EnsureLinesValid(lines);
        decimal subtotal = 0m;
        foreach (PrescriptionLine line in lines)
        {
            decimal amount = line.Quantity * line.UnitPrice;
            if (line.Quantity >= PricingRules.BulkQuantity)
            {
                amount -= amount * PricingRules.BulkRate;
            }

            subtotal += amount;
        }

        return subtotal;
    }

    /// <summary>Calculates the regular-patient discount.</summary>
    /// <param name="amount">Nonnegative amount.</param>
    /// <param name="isRegularPatient">Whether the regular-patient discount applies.</param>
    /// <returns>The calculated amount.</returns>
    public static decimal ClientDiscountOf(decimal amount, bool isRegularPatient)
    {
        DocumentGuards.EnsureAmountValid(amount, nameof(amount));
        return isRegularPatient ? amount * PricingRules.RegularPatientRate : 0m;
    }

    /// <summary>Calculates the volume discount.</summary>
    /// <param name="amount">Nonnegative amount.</param>
    /// <returns>The calculated amount.</returns>
    public static decimal VolumeDiscountOf(decimal amount)
    {
        DocumentGuards.EnsureAmountValid(amount, nameof(amount));
        if (amount > PricingRules.LargePrescriptionFrom)
        {
            return amount * PricingRules.LargePrescriptionRate;
        }

        if (amount > PricingRules.MediumPrescriptionFrom)
        {
            return amount * PricingRules.MediumPrescriptionRate;
        }

        return 0m;
    }

    /// <summary>Calculates the coupon discount.</summary>
    /// <param name="amount">Nonnegative amount.</param>
    /// <param name="couponCode">Coupon code, or null when no coupon is supplied.</param>
    /// <returns>The calculated amount.</returns>
    public static decimal CouponDiscountOf(decimal amount, string? couponCode)
    {
        DocumentGuards.EnsureAmountValid(amount, nameof(amount));
        return couponCode switch
        {
            PricingRules.SaleCoupon => amount * PricingRules.CouponSaleRate,
            PricingRules.FixedCoupon when amount > PricingRules.CouponFixedFrom
                => PricingRules.CouponFixed,
            _ => 0m,
        };
    }

    /// <summary>Calculates the Sunday discount.</summary>
    /// <param name="amount">Nonnegative amount.</param>
    /// <param name="createdAt">Document creation date.</param>
    /// <returns>The calculated amount.</returns>
    public static decimal WeekendDiscountOf(decimal amount, DateOnly createdAt)
    {
        DocumentGuards.EnsureAmountValid(amount, nameof(amount));
        return createdAt.DayOfWeek == DayOfWeek.Sunday ? amount * PricingRules.WeekendRate : 0m;
    }

    /// <summary>Calculates the delivery fee.</summary>
    /// <param name="amount">Nonnegative amount.</param>
    /// <param name="deliveryPrice">Nonnegative delivery price.</param>
    /// <returns>The calculated amount.</returns>
    public static decimal DeliveryFeeOf(decimal amount, decimal deliveryPrice)
    {
        DocumentGuards.EnsureAmountValid(amount, nameof(amount));
        DocumentGuards.EnsureAmountValid(deliveryPrice, nameof(deliveryPrice));
        return amount < PricingRules.FreeDeliveryFrom ? deliveryPrice : 0m;
    }

    /// <summary>Returns the next state without modifying the input.</summary>
    /// <param name="current">Current supported document state.</param>
    /// <param name="total">Nonnegative calculated total.</param>
    /// <returns>The next state according to the inherited rules.</returns>
    public static DocumentStatus NextStatusOf(DocumentStatus current, decimal total)
    {
        DocumentGuards.EnsureStatusValid(current);
        DocumentGuards.EnsureAmountValid(total, nameof(total));
        return current switch
        {
            DocumentStatus.New when total > 0m => DocumentStatus.Paid,
            DocumentStatus.Paid => DocumentStatus.Dispensed,
            _ => current,
        };
    }
}
