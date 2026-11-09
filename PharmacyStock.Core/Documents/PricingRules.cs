namespace PharmacyStock.Core.Documents;

public static class PricingRules
{
    public const int BulkQuantity = 10;

    public const decimal BulkRate = 0.05m;

    public const decimal RegularPatientRate = 0.07m;

    public const decimal LargePrescriptionFrom = 5000m;

    public const decimal LargePrescriptionRate = 0.10m;

    public const decimal MediumPrescriptionFrom = 2000m;

    public const decimal MediumPrescriptionRate = 0.05m;

    public const decimal WeekendRate = 0.02m;

    public const decimal CouponSaleRate = 0.10m;

    public const decimal CouponFixed = 200m;

    public const decimal CouponFixedFrom = 1000m;

    public const decimal FreeDeliveryFrom = 1000m;

    public const string SaleCoupon = "SALE10";

    public const string FixedCoupon = "MINUS200";
}
