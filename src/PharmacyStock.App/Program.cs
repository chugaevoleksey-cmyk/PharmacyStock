using System.Globalization;
using PharmacyStock.Core.Application;
using PharmacyStock.Infrastructure;
namespace PharmacyStock.App;

internal static class Program
{
    public static void Main(string[] args)
    {
        Directory.CreateDirectory("runtime");
        string path = "runtime/stock.json";
        if (!File.Exists(path))
        {
            File.Copy(Path.Combine(AppContext.BaseDirectory, "data/seed.json"), path);
        }
        var repository = new JsonStockRepository(path);
        var log = new FileAppLog("runtime/app.log");
        var service = new StockService(repository, new SystemClock(), new DispensingPolicy(), log);
        var session = new ConsoleSession(service, repository, log);
        Run(args, session);
    }
    private static void Run(string[] args, ConsoleSession session)
    {
        if (args.Contains("--demo", StringComparer.Ordinal))
        {
            string future = DateOnly.FromDateTime(DateTime.Today).AddDays(7).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            string script = $"load\nreceive 1 M002 20 {future}\n            create 19 1 M002 2\nstate 19 approve\ndispense M002 2 19\ndispense M002 1 19\nexpiring 30\nsave\nexit\n";
            using var input = new StringReader(script);
            session.Run(input, Console.Out);
            return;
        }
        using var startup = new StringReader("load");
        session.Run(startup, Console.Out);
        session.Run(Console.In, Console.Out);
    }
}
