using EMA.AssetManager.Domain.Data;
using EMA.AssetManager.Domain.Entities;
using EMA.AssetManager.Services.Dtos.Items;
using EMA.AssetManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EMA.AssetManager.Services.Services;

public class ItemService : IItemService
{
    private readonly AssertManagerDbContext _dbContext;

    public ItemService(AssertManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<ItemDto>> GetItemsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Items
            .Include(i => i.Category) // عشان نجيب اسم الفئة
            .AsNoTracking()
            .OrderBy(i => i.Name)
            .Select(i => new ItemDto
            {
                Id = i.Id,
                Code = i.Code,
                Name = i.Name,
                Quantity = i.Quantity,
                Unit = i.Unit,
                CategoryId = i.CategoryId,
                CategoryName = i.Category.Name
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<ItemDto?> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Items
            .Include(i => i.Category)
            .Where(i => i.Id == id)
            .Select(i => new ItemDto
            {
                Id = i.Id,
                Code = i.Code,
                Name = i.Name,
                Quantity = i.Quantity,
                Unit = i.Unit,
                CategoryId = i.CategoryId,
                CategoryName = i.Category.Name
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ItemDto> CreateItemAsync(CreateItemDto dto, CancellationToken cancellationToken = default)
    {
        // 1. التأكد من وجود الفئة
        var categoryExists = await _dbContext.Categories.AnyAsync(c => c.Id == dto.CategoryId, cancellationToken);
        if (!categoryExists)
            throw new Exception("الفئة المختارة غير موجودة.");

        // 2. التأكد من عدم تكرار الكود
        if (await _dbContext.Items.AnyAsync(i => i.Code == dto.Code, cancellationToken))
            throw new Exception($"الكود '{dto.Code}' مستخدم بالفعل.");

        var item = new Item
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Code = dto.Code,
            CategoryId = dto.CategoryId,
            Quantity = dto.Quantity,
            Unit = dto.Unit,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _dbContext.Items.Add(item);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // بنرجع الـ Dto عشان الـ UI يستقبله
        // ملحوظة: بنجيب اسم الفئة عشان العرض
        var categoryName = await _dbContext.Categories
            .Where(c => c.Id == dto.CategoryId)
            .Select(c => c.Name)
            .FirstAsync(cancellationToken);

        return new ItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Code = item.Code,
            Quantity = item.Quantity,
            Unit = item.Unit,
            CategoryId = item.CategoryId,
            CategoryName = categoryName
        };
    }

    public async Task<ItemDto> UpdateItemAsync(Guid id, UpdateItemDto dto, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.Items.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        if (item == null) throw new Exception("الصنف غير موجود.");

        // التأكد من الكود لو اتغير
        if (item.Code != dto.Code && await _dbContext.Items.AnyAsync(i => i.Code == dto.Code, cancellationToken))
            throw new Exception($"الكود '{dto.Code}' مستخدم بالفعل.");

        item.Name = dto.Name;
        item.Code = dto.Code;
        item.CategoryId = dto.CategoryId;
        item.Quantity = dto.Quantity;
        item.Unit = dto.Unit;

        // تحديث تاريخ التعديل (لو الـ Entity فيه LastModified)
        // item.LastModified = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        // إرجاع البيانات المحدثة
        return await GetItemByIdAsync(id, cancellationToken) ?? throw new Exception("خطأ أثناء استرجاع البيانات");
    }

    public async Task DeleteItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.Items.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        if (item == null) throw new Exception("الصنف غير موجود.");

        _dbContext.Items.Remove(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<string> GenerateNextCodeAsync(CancellationToken cancellationToken = default)
    {
        // دالة بسيطة لتوليد كود تلقائي: ITEM-001, ITEM-002...
        var count = await _dbContext.Items.CountAsync(cancellationToken);
        return $"ITEM-{(count + 1):D4}";
    }
}
