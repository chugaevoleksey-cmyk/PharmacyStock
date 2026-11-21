namespace PharmacyStock.Legacy;
public enum ClientKind { Other, Regular, Vip, Staff }
public enum DocState { Unknown, Draft, New, Paid, Closed }
public static class LegacyKinds
{
    public static ClientKind Client(string? value) => value switch
    {
        "regular" => ClientKind.Regular, "vip" => ClientKind.Vip,
        "staff" => ClientKind.Staff, _ => ClientKind.Other
    };
    public static DocState State(string? value) => value switch
    {
        "new" => DocState.New, "paid" => DocState.Paid,
        "draft" => DocState.Draft, "closed" => DocState.Closed,
        _ => DocState.Unknown
    };
}
