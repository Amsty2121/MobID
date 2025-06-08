using MobID.MainGateway.Models.Entities;
using MobID.MainGateway.Models.Enums;

namespace MobID.MainGateway.Models.Dtos;

public class AccessTypeDto
{
    public Guid Id { get; }
    public string Name { get; }
    public string Description { get; }
    public AccessTypeCode AccessTypeCode { get; set; }


    public AccessTypeDto(AccessType at)
    {
        Id = at.Id;
        Name = at.Name;
        Description = at.Description;
        AccessTypeCode = at.Code;
    }
}
