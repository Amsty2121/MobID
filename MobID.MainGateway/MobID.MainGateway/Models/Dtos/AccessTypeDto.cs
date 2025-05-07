using MobID.MainGateway.Models.Entities;

namespace MobID.MainGateway.Models.Dtos;

public class AccessTypeDto
{
    public Guid Id { get; }
    public string Name { get; }
    public string Description { get; }

    public AccessTypeDto(AccessType at)
    {
        Id = at.Id;
        Name = at.Name;
        Description = at.Description;
    }
}
