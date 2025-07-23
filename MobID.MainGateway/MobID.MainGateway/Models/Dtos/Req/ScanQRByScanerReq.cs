namespace MobID.MainGateway.Models.Dtos.Req
{
    public class ScanQRByScanerReq
    {
        public Guid OrganizationId { get; set; }
        public Guid AccessId { get; set; }
        public string Payload { get; set; }
    }
}
