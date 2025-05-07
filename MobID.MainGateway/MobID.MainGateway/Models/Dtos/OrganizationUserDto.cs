using MobID.MainGateway.Models.Entities;

namespace MobID.MainGateway.Models.Dtos;

public class OrganizationUserDto
{
    public Guid Id { get; }
    public Guid OrganizationId { get; }
    public Guid UserId { get; }
    public string UserName { get; }
    public string Role { get; }    

    public OrganizationUserDto(OrganizationUser ou)
    {
        Id = ou.Id;
        OrganizationId = ou.OrganizationId;
        UserId = ou.UserId;
        UserName = ou.User?.Username ?? "Unknown";
        Role = ou.Role.ToString(); 
    }
}
