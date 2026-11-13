namespace PharmacyStock.Legacy;
public static class LegacyDiscountPolicy
{
    public static decimal For(decimal sum, ClientKind kind, int done)
    {
        decimal discount = 0m;
        if (kind == ClientKind.Vip) discount = sum * PricingRules.VipRate;
        else if (kind == ClientKind.Staff) discount = sum * PricingRules.StaffRate;
        else if (done > PricingRules.LoyalOrders) discount = sum * PricingRules.LoyalRate;
        if (discount > PricingRules.MaxDiscount) discount = PricingRules.MaxDiscount;
        return discount;
    }
}
