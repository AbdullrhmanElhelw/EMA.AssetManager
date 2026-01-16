using EMA.AssetManager.Services.Dtos.Items;

namespace EMA.AssetManager.Services.Interfaces;

public interface IItemService
{
    Task<IReadOnlyCollection<ItemDto>> GetItemsAsync(CancellationToken cancellationToken = default);
    Task<ItemDto?> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ItemDto> CreateItemAsync(CreateItemDto dto, CancellationToken cancellationToken = default);
    Task<ItemDto> UpdateItemAsync(Guid id, UpdateItemDto dto, CancellationToken cancellationToken = default);
    Task DeleteItemAsync(Guid id, CancellationToken cancellationToken = default);

    // دالة مساعدة لاقتراح كود جديد للصنف (Optional)
    Task<string> GenerateNextCodeAsync(CancellationToken cancellationToken = default);
}
