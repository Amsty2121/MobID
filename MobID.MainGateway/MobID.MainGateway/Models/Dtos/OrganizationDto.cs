using MobID.MainGateway.Models.Entities;

namespace MobID.MainGateway.Models.Dtos
{
    public class OrganizationDto
    {
        public Guid Id { get; }
        public string Name { get; }
        public Guid OwnerId { get; }
        public string OwnerName { get; }

        public OrganizationDto(Organization organization)
        {
            Id = organization.Id;
            Name = organization.Name;
            OwnerId = organization.OwnerId;
            OwnerName = string.Join(" | ", organization.Owner?.Username, organization.Owner?.Id) ?? "Unknown";
        }
    }
}
