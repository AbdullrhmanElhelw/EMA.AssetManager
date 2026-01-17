using EMA.AssetManager.Domain.Data;
using EMA.AssetManager.Services.Dtos.Categories;
using EMA.AssetManager.Services.Dtos.Categories.Create;
using EMA.AssetManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EMA.AssetManager.Services.Interfaces
{
    public class CategoryService : ICategoryService
    {
        private readonly AssertManagerDbContext _dbContext;

        public CategoryService(AssertManagerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default)
        {
            // التحقق من عدم وجود فئة بنفس الاسم
            if (await _dbContext.Categories.AnyAsync(c => c.Name == dto.Name, cancellationToken))
            {
                throw new InvalidOperationException($"الفئة '{dto.Name}' موجودة بالفعل.");
            }

            var category = new Domain.Entities.Category
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                ItemsCount = 0
            };
        }

        public async Task<IReadOnlyCollection<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken = default)
        {
            var categories = await _dbContext.Categories
                .OrderBy(c => c.Name)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    ItemsCount = c.Items.Count
                })
                .ToListAsync(cancellationToken);

            return categories;
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Categories
                .Where(c => c.Id == id)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    ItemsCount = c.Items.Count
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<CategoryDto> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto, CancellationToken cancellationToken = default)
        {
            var category = await _dbContext.Categories
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            if (category == null)
            {
                throw new KeyNotFoundException($"الفئة ذات المعرف {id} غير موجودة.");
            }

            // التحقق من عدم وجود فئة أخرى بنفس الاسم
            if (await _dbContext.Categories.AnyAsync(c => c.Name == dto.Name && c.Id != id, cancellationToken))
            {
                throw new InvalidOperationException($"الفئة '{dto.Name}' موجودة بالفعل.");
            }

            category.Name = dto.Name;
            category.IsActive = dto.IsActive;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                ItemsCount = category.Items.Count
            };
        }

        public async Task DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var category = await _dbContext.Categories
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            if (category == null)
            {
                throw new KeyNotFoundException($"الفئة ذات المعرف {id} غير موجودة.");
            }

            // التحقق مما إذا كانت الفئة تحتوي على عناصر
            if (category.Items.Any())
            {
                throw new InvalidOperationException("لا يمكن حذف فئة تحتوي على عناصر.");
            }

            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> CategoryExistsAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Categories
                .AnyAsync(c => c.Name == name, cancellationToken);
        }
    }
}
