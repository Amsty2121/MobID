namespace MobID.MainGateway.Models.Dtos;

public class ScanFullDto
{
    public Guid Id { get; set; }

    public Guid QrCodeId { get; set; }
    public Guid AccessId { get; set; }
    public string AccessName { get; set; } = null!;
    public Guid OrganizationId { get; set; }
    public string OrganizationName { get; set; } = null!;

    public Guid ScannedByUserId { get; set; }
    public string ScannedByUserName { get; set; } = null!;
    public Guid ScannedForUserId { get; set; }
    public string ScannedForUserName { get; set; } = null!;

    public DateTime ScannedAt { get; set; }

    public ScanFullDto(Scan scan)
    {
        Id = scan.Id;
        QrCodeId = scan.QrCodeId;

        AccessId = scan.QrCode!.AccessId;
        AccessName = scan.QrCode!.Access.Name;
        OrganizationId = scan.QrCode.Access.OrganizationId;
        OrganizationName = scan.QrCode.Access.Organization.Name;

        ScannedByUserId = scan.ScannedByUserId;
        ScannedByUserName = scan.ScannedByUser!.Username;
        ScannedForUserId = scan.ScannedForUserId;
        ScannedForUserName = scan.ScannedForUser!.Username;

        ScannedAt = scan.CreatedAt;
    }
}
