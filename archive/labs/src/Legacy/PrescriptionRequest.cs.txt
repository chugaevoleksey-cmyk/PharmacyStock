namespace PharmacyStock.Legacy;
public sealed record PrescriptionRequest(int DocId, LegacyPatient Patient,
    List<LegacyPrescriptionLine>? Items, DocState State, string Currency,
    DateTime CreatedAt, bool SendMail);
