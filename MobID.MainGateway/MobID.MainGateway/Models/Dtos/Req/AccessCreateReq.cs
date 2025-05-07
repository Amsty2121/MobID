namespace MobID.MainGateway.Models.Dtos.Req
{
    public class AccessCreateReq
    {
        public Guid OrganizationId { get; set; }
        public Guid AccessTypeId { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}
