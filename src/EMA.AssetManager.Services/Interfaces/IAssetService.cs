using EMA.AssetManager.Domain.Enums;
using EMA.AssetManager.Services.Dtos.Assets;

public interface IAssetService
{
    Task<IReadOnlyCollection<AssetDto>> GetAssetsAsync(CancellationToken cancellationToken = default);
    Task<AssetDto?> GetAssetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AssetDto> CreateAssetAsync(CreateAssetDto dto, CancellationToken cancellationToken = default);
    Task<AssetDto> UpdateAssetAsync(Guid id, UpdateAssetDto dto, CancellationToken cancellationToken = default);
    Task DeleteAssetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AssetDto?> GetAssetByIdAsync(Guid id);
    Task UpdateAssetStatusAsync(Guid assetId, AssetStatus newStatus);

}
