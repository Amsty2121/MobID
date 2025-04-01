namespace MobID.MainGateway.Models.Dtos.Req
{
    public class ScanCreateReq
    {
        public Guid AccessId { get; set; }
        public Guid ScannedById { get; set; }
        public Guid? QrCodeId { get; set; }
    }
}
