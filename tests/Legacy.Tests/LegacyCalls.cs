using PharmacyStock.Legacy;
namespace PharmacyStock.Legacy.Tests;
// Only API adaptations live here. Test cases and expected results remain unchanged.
internal static class LegacyCalls
{
    internal static string Handle(LegacyPrescriptionProcessor sut, int docId, int clientId,
        string clientName, string? clientMail, string clientKind, int clientDone,
        List<LegacyPrescriptionLine>? items, string state, string currency, DateTime createdAt, bool sendMail)
        => sut.Handle(docId, clientId, clientName, clientMail, LegacyKinds.Client(clientKind), clientDone,
            items, LegacyKinds.State(state), currency, createdAt, sendMail);
    internal static decimal Preview(LegacyPrescriptionProcessor sut, string kind, int done, List<LegacyPrescriptionLine> items)
        => sut.Preview(LegacyKinds.Client(kind), done, items);
    internal static string Describe(LegacyPrescriptionProcessor sut, LegacyPatient patient) => sut.DescribeClient(patient);
    internal static LegacyPatient Patient(string kind) => new() { Name = "  іван  ", Email = "IVAN@MAIL.COM", Kind = LegacyKinds.Client(kind), DoneCount = 12, SinceUtc = new DateTime(2020, 1, 1) };
}
