namespace MobID.MainGateway.Models.Enums
{
    public enum ScanMode
    {
        UserScansQr,      // Utilizatorul scanează un cod QR (ex: bilet de transport)
        VerifierScansUser // Un verificator scanează codul QR al utilizatorului (ex: confirmare identitate)
    }
}
