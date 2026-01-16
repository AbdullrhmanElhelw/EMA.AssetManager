using EMA.AssetManager.Services.Dtos.Categories;
using EMA.AssetManager.Services.Dtos.Categories.Create;

namespace EMA.AssetManager.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken = default);
        Task<CategoryDto?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<CategoryDto> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto, CancellationToken cancellationToken = default);
        Task DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> CategoryExistsAsync(string name, CancellationToken cancellationToken = default);
    }
}
