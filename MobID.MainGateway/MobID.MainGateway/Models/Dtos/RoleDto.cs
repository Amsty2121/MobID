using MobID.MainGateway.Models.Entities;
using System;

namespace MobID.MainGateway.Models.Dtos.Rsp
{
    public class RoleDto
    {
        public Guid Id { get; }
        public string Name { get; }
        public string Description { get; }
        public bool IsDeleted { get; }

        public RoleDto(Role role)
        {
            Id = role.Id;
            Name = role.Name;
            Description = role.Description;
            IsDeleted = role.DeletedAt != null;
        }
    }
}
