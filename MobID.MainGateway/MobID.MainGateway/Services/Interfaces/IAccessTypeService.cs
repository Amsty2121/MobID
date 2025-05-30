﻿using MobID.MainGateway.Models.Dtos;

namespace MobID.MainGateway.Services.Interfaces;

public interface IAccessTypeService
{
    Task<List<AccessTypeDto>> GetAllTypesAsync(CancellationToken ct = default);
}
