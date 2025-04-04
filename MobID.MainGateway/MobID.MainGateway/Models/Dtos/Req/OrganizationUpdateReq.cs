namespace MobID.MainGateway.Models.Dtos.Req
{
    public class OrganizationUpdateReq
    {
        public Guid OrganizationId { get; set; }
        public string? Name { get; set; }
        public Guid? OwnerId { get; set; }
    }
}
