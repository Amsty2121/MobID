using MobID.MainGateway.Models.Entities;

namespace MobID.MainGateway.Models.Dtos
{
    public class OrganizationUserDto
    {
        public Guid Id { get; }
        public Guid OrganizationId { get; }
        public Guid UserId { get; }
        public string UserName { get; }

        public OrganizationUserDto(OrganizationUser orgUser)
        {
            Id = orgUser.Id;
            OrganizationId = orgUser.OrganizationId;
            UserId = orgUser.UserId;
            UserName = orgUser.User?.Username ?? "Unknown";
        }
    }
}
