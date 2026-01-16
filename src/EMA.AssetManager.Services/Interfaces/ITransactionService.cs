using EMA.AssetManager.Services.Dtos.Assets;
using EMA.AssetManager.Services.Dtos.Transactions;

public interface ITransactionService
{
    Task IssueAssetAsync(IssueAssetDto dto);

    Task ReturnAssetAsync(ReturnAssetDto dto);
    Task<List<AssetTransactionDto>> GetAssetHistoryAsync(Guid assetId);

}