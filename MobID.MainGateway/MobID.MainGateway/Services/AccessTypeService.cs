using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Entities;
using MobID.MainGateway.Repo.Interfaces;
using MobID.MainGateway.Services.Interfaces;

namespace MobID.MainGateway.Services;

public class AccessTypeService : IAccessTypeService
{
    private readonly IGenericRepository<AccessType> _repo;
    public AccessTypeService(IGenericRepository<AccessType> repo) => _repo = repo;

    public async Task<List<AccessTypeDto>> GetAllTypes(CancellationToken ct = default)
    {
        var entities = await _repo.GetWhere(at => at.DeletedAt == null, ct);
        return entities.Select(at => new AccessTypeDto(at)).ToList();
    }
}
