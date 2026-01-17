using EMA.AssetManager.Domain.Data;
using EMA.AssetManager.Domain.Entities;
using EMA.AssetManager.Domain.Enums;
using EMA.AssetManager.Services.Dtos.Assets;
using EMA.AssetManager.Services.Dtos.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EMA.AssetManager.Services.Interfaces;

public class TransactionService : ITransactionService
{
    private readonly AssertManagerDbContext _dbContext;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(
        AssertManagerDbContext dbContext,
        ILogger<TransactionService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task IssueAssetAsync(IssueAssetDto dto)
    {
        // استخدم CreateExecutionStrategy للتعامل مع المعاملات
        var executionStrategy = _dbContext.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
            // استخدام using بدلاً من await using لتجنب المشكلة
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                // التحقق من وجود الأصل
                var asset = await _dbContext.Assets
                    .Include(a => a.Item)
                    .FirstOrDefaultAsync(a => a.Id == dto.AssetId);

                if (asset == null)
                    throw new Exception($"الأصل غير موجود (ID: {dto.AssetId})");

                // التحقق من حالة الأصل
                if (asset.Status != AssetStatus.Available)
                    throw new Exception($"الأصل غير متاح للصرف. الحالة الحالية: {asset.Status}");

                // التحقق من أن المخزن موجود
                var warehouse = await _dbContext.Warehouses
                    .FirstOrDefaultAsync(w => w.Id == asset.WarehouseId);

                if (warehouse == null)
                    throw new Exception($"المخزن غير موجود (ID: {asset.WarehouseId})");

                // إنشاء معاملة الصرف
                var assetTransaction = new AssetTransaction
                {
                    Id = Guid.NewGuid(),
                    AssetId = dto.AssetId,
                    TransactionType = TransactionType.Issue,
                    TransactionDate = dto.Date,
                    RecipientName = dto.RecipientName?.Trim(),
                    WarehouseId = asset.WarehouseId,
                    Notes = dto.Notes?.Trim(),
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };

                // تحديث حالة الأصل
                asset.Status = AssetStatus.InUse;

                // حفظ التغييرات
                _dbContext.AssetTransactions.Add(assetTransaction);
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                _logger.LogInformation("تم صرف الأصل {AssetId} إلى {RecipientName} بنجاح",
                    dto.AssetId, dto.RecipientName);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "فشل صرف الأصل {AssetId}", dto.AssetId);
                throw new Exception($"فشل صرف الأصل: {ex.Message}");
            }
        });
    }

    public async Task ReturnAssetAsync(ReturnAssetDto dto)
    {
        // استخدم CreateExecutionStrategy للتعامل مع المعاملات
        var executionStrategy = _dbContext.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                // التحقق من وجود الأصل
                var asset = await _dbContext.Assets
                    .Include(a => a.Item)
                    .FirstOrDefaultAsync(a => a.Id == dto.AssetId);

                if (asset == null)
                    throw new Exception($"الأصل غير موجود (ID: {dto.AssetId})");

                // التحقق من أن الأصل مستخدم حالياً
                if (asset.Status != AssetStatus.InUse)
                    throw new Exception($"الأصل غير مستخدم حالياً. الحالة الحالية: {asset.Status}");

                // التحقق من وجود المخزن المستلم
                var returnWarehouse = await _dbContext.Warehouses
                    .FirstOrDefaultAsync(w => w.Id == dto.ReturnToWarehouseId);

                if (returnWarehouse == null)
                    throw new Exception($"المخزن المستلم غير موجود (ID: {dto.ReturnToWarehouseId})");

                // إنشاء معاملة الإرجاع
                var assetTransaction = new AssetTransaction
                {
                    Id = Guid.NewGuid(),
                    AssetId = dto.AssetId,
                    TransactionType = TransactionType.Return,
                    TransactionDate = dto.Date,
                    RecipientName = null,
                    WarehouseId = dto.ReturnToWarehouseId,
                    Notes = dto.Notes?.Trim(),
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };

                // تحديث حالة ومكان الأصل
                asset.Status = dto.NewStatus;
                asset.WarehouseId = dto.ReturnToWarehouseId;

                // حفظ التغييرات
                _dbContext.AssetTransactions.Add(assetTransaction);
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                _logger.LogInformation("تم إرجاع الأصل {AssetId} إلى المخزن {WarehouseId} بنجاح",
                    dto.AssetId, dto.ReturnToWarehouseId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "فشل إرجاع الأصل {AssetId}", dto.AssetId);
                throw new Exception($"فشل إرجاع الأصل: {ex.Message}");
            }
        });
    }


    public async Task<List<AssetTransactionDto>> GetAssetHistoryAsync(Guid assetId)
    {

        var history = await _dbContext.AssetTransactions
            .Include(t => t.Warehouse)
            .Where(t => t.AssetId == assetId)
            .OrderByDescending(t => t.TransactionDate) // الأحدث فوق
            .Select(t => new AssetTransactionDto
            {
                Id = t.Id,
                TransactionType = t.TransactionType,
                TransactionDate = t.TransactionDate,
                RecipientName = t.RecipientName,
                WarehouseName = t.Warehouse != null ? t.Warehouse.Name : "-",
                Notes = t.Notes
            })
            .ToListAsync();

        return history;
    }
}