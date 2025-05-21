using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Entities;
using MobID.MainGateway.Repo.Interfaces;
using MobID.MainGateway.Services.Interfaces;

namespace MobID.MainGateway.Services;

public class AccessTypeService : IAccessTypeService
{
    private readonly IGenericRepository<AccessType> _repo;
    public AccessTypeService(IGenericRepository<AccessType> repo) => _repo = repo;

    /// <summary>
    /// Returnează toate tipurile de acces active.
    /// </summary>
    public async Task<List<AccessTypeDto>> GetAllTypesAsync(CancellationToken ct = default)
    {
        var entities = await _repo.GetWhere(at => at.DeletedAt == null, ct);
        return entities.Select(at => new AccessTypeDto(at)).ToList();
    }

    /// <summary>
    /// Returnează un tip de acces după ID (sau null dacă nu există).
    /// </summary>
    public async Task<AccessTypeDto?> GetTypeByIdAsync(Guid typeId, CancellationToken ct = default)
    {
        var at = await _repo.GetById(typeId, ct);
        return at == null ? null : new AccessTypeDto(at);
    }
}