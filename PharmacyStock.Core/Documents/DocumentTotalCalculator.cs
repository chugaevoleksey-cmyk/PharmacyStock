namespace PharmacyStock.Core.Documents;

public static class DocumentTotalCalculator
{
    public static TotalResult Calculate(TotalRequest request)
    {
        DocumentGuards.EnsureRequestValid(request);
        decimal total = SubtotalOf(request.Lines);
        total -= ClientDiscountOf(total, request.Patient.IsRegular);
        total -= VolumeDiscountOf(total);
        total -= CouponDiscountOf(total, request.CouponCode);
        total -= WeekendDiscountOf(total, request.CreatedAt);
        total += DeliveryFeeOf(total, request.DeliveryPrice);
        total = request.Status == DocumentStatus.Cancelled ? 0m : Math.Round(Math.Max(total, 0m), 2);
        return new TotalResult { Total = total, Status = NextStatusOf(request.Status, total) };
    }

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

    public static decimal ClientDiscountOf(decimal amount, bool isRegularPatient)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        return isRegularPatient ? amount * PricingRules.RegularPatientRate : 0m;
    }

    public static decimal VolumeDiscountOf(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
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

    public static decimal CouponDiscountOf(decimal amount, string? couponCode)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        return couponCode switch
        {
            PricingRules.SaleCoupon => amount * PricingRules.CouponSaleRate,
            PricingRules.FixedCoupon when amount > PricingRules.CouponFixedFrom
                => PricingRules.CouponFixed,
            _ => 0m,
        };
    }

    public static decimal WeekendDiscountOf(decimal amount, DateOnly createdAt)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        return createdAt.DayOfWeek == DayOfWeek.Sunday ? amount * PricingRules.WeekendRate : 0m;
    }

    public static decimal DeliveryFeeOf(decimal amount, decimal deliveryPrice)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        ArgumentOutOfRangeException.ThrowIfNegative(deliveryPrice);
        return amount < PricingRules.FreeDeliveryFrom ? deliveryPrice : 0m;
    }

    public static DocumentStatus NextStatusOf(DocumentStatus current, decimal total)
    {
        DocumentGuards.EnsureStatusValid(current);
        ArgumentOutOfRangeException.ThrowIfNegative(total);
        return current switch
        {
            DocumentStatus.New when total > 0m => DocumentStatus.Paid,
            DocumentStatus.Paid => DocumentStatus.Dispensed,
            _ => current,
        };
    }
}
