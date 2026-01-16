using EMA.AssetManager.Services.Dtos.Warehouses;

namespace EMA.AssetManager.Services.Interfaces;

public interface IWarehouseService
{
    Task<IReadOnlyCollection<WarehouseDto>> GetWarehousesAsync(CancellationToken cancellationToken = default);
    Task<WarehouseDto?> GetWarehouseByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WarehouseDto> CreateWarehouseAsync(CreateWarehouseDto dto, CancellationToken cancellationToken = default);
    Task<WarehouseDto> UpdateWarehouseAsync(Guid id, UpdateWarehouseDto dto, CancellationToken cancellationToken = default);
    Task DeleteWarehouseAsync(Guid id, CancellationToken cancellationToken = default);
}