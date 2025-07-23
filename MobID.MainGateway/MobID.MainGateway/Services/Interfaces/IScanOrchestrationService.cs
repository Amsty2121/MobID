using MobID.MainGateway.Models.Dtos;

namespace MobID.MainGateway.Services.Interfaces;

public interface IScanOrchestrationService
{
    Task<ScanDto> HandleQrScanAsync(string qrRawValue, Guid scanningUserId, CancellationToken ct = default);
}
