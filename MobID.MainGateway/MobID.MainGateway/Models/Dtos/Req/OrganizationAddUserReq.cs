namespace MobID.MainGateway.Models.Dtos.Req
{
    public class OrganizationAddUserReq
    {
        public Guid UserId { get; set; }
        public string? Role { get; set; } 
    }
}
