namespace MobID.MainGateway.Models.Enums;

public enum QrCodeType
{
    InviteToOrganization = 0, // daca scanezi acest qr - te adaugi in organizatie
    ShareAccess = 1, // daca scanezi acest qr - atunci tu nu intri in organizatie dar iti asigneaza tie acest access
    Access = 2 // daca scanezi acest qr - atunci to vei confirma un access
}