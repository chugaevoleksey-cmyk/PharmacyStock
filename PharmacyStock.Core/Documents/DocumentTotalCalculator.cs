namespace PharmacyStock.Core.Documents;

public static class DocumentTotalCalculator
{
    public static decimal Process(
        int documentId, string patientName, string patientEmail,
        bool isRegularPatient, IReadOnlyList<PrescriptionLine> lines,
        DateOnly createdAt, string? couponCode, DocumentStatus status,
        bool printToConsole, decimal deliveryPrice, out DocumentStatus newStatus)
    {
        DocumentGuards.EnsureHeaderValid(documentId, patientName, patientEmail);
        DocumentGuards.EnsureStatusValid(status);
        ArgumentOutOfRangeException.ThrowIfNegative(deliveryPrice);
        decimal total = SubtotalOf(lines);
        total -= ClientDiscountOf(total, isRegularPatient);
        total -= VolumeDiscountOf(total);
        total -= CouponDiscountOf(total, couponCode);
        total -= WeekendDiscountOf(total, createdAt);
        total += DeliveryFeeOf(total, deliveryPrice);
        total = status == DocumentStatus.Cancelled ? 0m : Math.Round(Math.Max(total, 0m), 2);
        newStatus = NextStatusOf(status, total);
        if (printToConsole)
        {
            Console.WriteLine($"Рецепт № {documentId}; {patientName}; {total}; {newStatus}");
        }

        return total;
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
