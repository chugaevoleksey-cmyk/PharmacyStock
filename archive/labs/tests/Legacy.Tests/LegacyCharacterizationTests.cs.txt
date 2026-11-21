using System.Globalization;
using PharmacyStock.Legacy;
using Xunit;
namespace PharmacyStock.Legacy.Tests;
public sealed class LegacyCharacterizationTests : IDisposable
{
    private readonly CultureInfo _original = CultureInfo.CurrentCulture;
    public LegacyCharacterizationTests() => CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    public void Dispose() => CultureInfo.CurrentCulture = _original;
    private static List<LegacyPrescriptionLine> TwoLines() => new()
    {
        new() { Code = "A-1", Qty = 2, Price = 150m },
        new() { Code = "B-2", Qty = 1, Price = 400m }
    };
    private static string Report(LegacyPrescriptionProcessor sut, List<LegacyPrescriptionLine>? items,
        string state = "new", string? mail = "a@b.c", string kind = "regular", int done = 0, bool send = false)
        => LegacyCalls.Handle(sut, 1001, 7, "Іван", mail, kind, done, items, state, "UAH", new DateTime(2026, 3, 1), send);
    [Theory]
    [InlineData(null, "new", "a@b.c", "ERR: null")]
    [InlineData("empty", "new", "a@b.c", "ERR: empty")]
    [InlineData("two", "draft", "a@b.c", "ERR: state")]
    [InlineData("two", "new", "bad", "ERR: mail")]
    [InlineData("two", "new", null, "ERR: mail")]
    [InlineData(null, "draft", "bad", "ERR: null")]
    [InlineData("empty", "draft", "bad", "ERR: empty")]
    [InlineData("two", "closed", "bad", "ERR: state")]
    [InlineData("two", "NEW", "a@b.c", "ERR: state")]
    public void Handle_BadInput_ReturnsFirstError(string? data, string state, string? mail, string expected)
    {
        var items = data switch { "two" => TwoLines(), "empty" => new List<LegacyPrescriptionLine>(), _ => null };
        var sut = new LegacyPrescriptionProcessor();
        Assert.Equal(expected, Report(sut, items, state, mail));
        Assert.Equal("", sut.DumpLog());
    }
    [Fact]
    public void Handle_Vip_ReturnsGoldenMaster()
    {
        var actual = Report(new LegacyPrescriptionProcessor(), TwoLines(), kind: "vip", done: 3);
        const string expected = "Документ #1001\nКлієнт: Іван\nA-1 x2 = 300.00 UAH\nB-2 x1 = 400.00 UAH\nЗнижка: 105.00 UAH\nДоставка: 60.00 UAH\nРазом: 655.00 UAH\n";
        Assert.Equal(expected, actual);
    }
    [Theory]
    [InlineData("regular", 0, 760)]
    [InlineData("vip", 0, 655)]
    [InlineData("staff", 0, 550)]
    [InlineData("regular", 11, 725)]
    [InlineData("regular", 10, 760)]
    [InlineData("VIP", 0, 760)]
    [InlineData("unknown", 11, 725)]
    [InlineData("vip", 99, 655)]
    [InlineData("staff", 99, 550)]
    public void Preview_KindPriority_IsPreserved(string kind, int done, decimal expected)
        => Assert.Equal(expected, LegacyCalls.Preview(new(), kind, done, TwoLines()));
    [Theory]
    [InlineData("vip", 10000, 9500)]
    [InlineData("staff", 10000, 9500)]
    [InlineData("regular", 10000, 9500)]
    public void Preview_DiscountHasCap(string kind, int price, decimal expected)
        => Assert.Equal(expected, LegacyCalls.Preview(new(), kind, 11, new() { new() { Qty = 1, Price = price } }));
    [Theory]
    [InlineData(999, 1059)]
    [InlineData(1000, 1000)]
    [InlineData(1001, 1001)]
    public void Preview_FreeShippingBoundary(int price, decimal expected)
        => Assert.Equal(expected, LegacyCalls.Preview(new(), "regular", 0, new() { new() { Qty = 1, Price = price } }));
    [Fact]
    public void Handle_PaidAndMail_KeepLogOrder()
    {
        var sut = new LegacyPrescriptionProcessor();
        Report(sut, TwoLines(), state: "paid", send: true);
        Report(sut, TwoLines());
        Assert.Equal("ok 1001\nmail -> a@b.c\nok 1001\n", sut.DumpLog());
    }
    [Fact]
    public void Describe_UsesCurrentYearAndCulture()
    {
        var years = DateTime.Now.Year - 2020;
        Assert.Equal("ІВАН [VIP] [ЛОЯЛЬНИЙ] <ivan@mail.com> стаж " + years,
            LegacyCalls.Describe(new(), LegacyCalls.Patient("vip")));
    }
    [Fact]
    public void Handle_UkrainianCulture_UsesDecimalComma()
    {
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("uk-UA");
        var actual = Report(new(), TwoLines(), kind: "vip");
        Assert.Equal("Документ #1001\nКлієнт: Іван\nA-1 x2 = 300,00 UAH\nB-2 x1 = 400,00 UAH\nЗнижка: 105,00 UAH\nДоставка: 60,00 UAH\nРазом: 655,00 UAH\n", actual);
    }
    [Fact]
    public void Handle_NegativePrice_IsNotRejected()
    {
        var actual = Report(new(), new() { new() { Code = "N", Qty = 1, Price = -100m } });
        Assert.Equal("Документ #1001\nКлієнт: Іван\nN x1 = -100.00 UAH\nЗнижка: 0.00 UAH\nДоставка: 60.00 UAH\nРазом: -40.00 UAH\n", actual);
    }
    [Fact]
    public void Preview_EmptyItems_StillChargesShipping()
        => Assert.Equal(60m, LegacyCalls.Preview(new(), "regular", 0, new()));
}
