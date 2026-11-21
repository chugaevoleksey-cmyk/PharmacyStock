namespace PharmacyStock.Legacy;
public static class LegacyShippingPolicy
{
    public static decimal For(decimal payable)
    {
        if (payable < PricingRules.FreeShipFrom) return PricingRules.ShipCost;
        return 0m;
    }
}
