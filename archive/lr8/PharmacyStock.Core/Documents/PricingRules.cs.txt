namespace PharmacyStock.Core.Documents;

/// <summary>Defines the inherited calculation rules.</summary>
public static class PricingRules
{
    /// <summary>The CurrencyDecimals rule.</summary>
    public const int CurrencyDecimals = 2;

    /// <summary>The BulkQuantity rule.</summary>
    public const int BulkQuantity = 10;

    /// <summary>The BulkRate rule.</summary>
    public const decimal BulkRate = 0.05m;

    /// <summary>The RegularPatientRate rule.</summary>
    public const decimal RegularPatientRate = 0.07m;

    /// <summary>The LargePrescriptionFrom rule.</summary>
    public const decimal LargePrescriptionFrom = 5000m;

    /// <summary>The LargePrescriptionRate rule.</summary>
    public const decimal LargePrescriptionRate = 0.10m;

    /// <summary>The MediumPrescriptionFrom rule.</summary>
    public const decimal MediumPrescriptionFrom = 2000m;

    /// <summary>The MediumPrescriptionRate rule.</summary>
    public const decimal MediumPrescriptionRate = 0.05m;

    /// <summary>The WeekendRate rule.</summary>
    public const decimal WeekendRate = 0.02m;

    /// <summary>The CouponSaleRate rule.</summary>
    public const decimal CouponSaleRate = 0.10m;

    /// <summary>The CouponFixed rule.</summary>
    public const decimal CouponFixed = 200m;

    /// <summary>The CouponFixedFrom rule.</summary>
    public const decimal CouponFixedFrom = 1000m;

    /// <summary>The FreeDeliveryFrom rule.</summary>
    public const decimal FreeDeliveryFrom = 1000m;

    /// <summary>The SaleCoupon rule.</summary>
    public const string SaleCoupon = "SALE10";

    /// <summary>The FixedCoupon rule.</summary>
    public const string FixedCoupon = "MINUS200";
}
