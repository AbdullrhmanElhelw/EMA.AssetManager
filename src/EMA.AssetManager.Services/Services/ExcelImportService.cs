using ClosedXML.Excel;
using EMA.AssetManager.Domain.Data;
using EMA.AssetManager.Domain.Entities;
using EMA.AssetManager.Domain.Enums;
using EMA.AssetManager.Services.Dtos.Import;
using Microsoft.EntityFrameworkCore;

namespace EMA.AssetManager.Services.Interfaces;

public class ExcelImportService
{
    private readonly AssertManagerDbContext _context;

    public ExcelImportService(AssertManagerDbContext context)
    {
        _context = context;
    }

    // ==========================================
    // 1. توليد القالب (Dropdowns: Items & Warehouses)
    // ==========================================
    public async Task<byte[]> GenerateTemplateAsync()
    {
        using var workbook = new XLWorkbook();

        // --- الشيت الرئيسي ---
        var worksheet = workbook.Worksheets.Add("Asset_Entry");

        // العناوين (رجعنا لاسم الصنف لأنه الأساس)
        worksheet.Cell(1, 1).Value = "Item Name (اسم الصنف)";
        worksheet.Cell(1, 2).Value = "Warehouse Name (اسم المخزن)";
        worksheet.Cell(1, 3).Value = "Barcode (الباركود)";
        worksheet.Cell(1, 4).Value = "Serial Number (السيريال)";

        // تنسيق الهيدر
        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#005c99"); // أزرق
        headerRow.Style.Font.FontColor = XLColor.White;

        // --- تجهيز البيانات ---

        // 1. نجيب الأصناف (Items) لأن هي دي اللي بتحدد نوع الأصل
        var itemsList = await _context.Items
            .Where(i => i.IsActive) // يفضل تجيب المفعل فقط
            .Select(i => i.Name)
            .OrderBy(n => n)
            .ToListAsync();

        // 2. نجيب المخازن
        var warehousesList = await _context.Warehouses
            .Where(w => w.IsActive)
            .Select(w => w.Name)
            .OrderBy(n => n)
            .ToListAsync();

        // تنبيه لو مفيش داتا
        if (itemsList.Count == 0) itemsList.Add("لا يوجد أصناف مسجلة - يجب إضافتها أولاً");
        if (warehousesList.Count == 0) warehousesList.Add("المخزن الرئيسي");

        // --- الشيت المخفي ---
        var dataSheet = workbook.Worksheets.Add("Reference_Data");
        dataSheet.Visibility = XLWorksheetVisibility.Hidden;

        // تعبئة الأصناف
        for (int i = 0; i < itemsList.Count; i++)
        {
            dataSheet.Cell(i + 1, 1).Value = itemsList[i];
        }
        // تعبئة المخازن
        for (int i = 0; i < warehousesList.Count; i++)
        {
            dataSheet.Cell(i + 1, 2).Value = warehousesList[i];
        }

        // --- تفعيل القوائم المنسدلة ---

        // القائمة الأولى: الأصناف
        var itemRange = worksheet.Range("A2:A2000");
        itemRange.SetDataValidation().List(dataSheet.Range(1, 1, itemsList.Count, 1), true);

        // القائمة الثانية: المخازن
        var whRange = worksheet.Range("B2:B2000");
        whRange.SetDataValidation().List(dataSheet.Range(1, 2, warehousesList.Count, 2), true);

        // تنسيق العرض
        worksheet.Columns().AdjustToContents();
        worksheet.Column(1).Width = 40; // الصنف عريض شوية
        worksheet.Column(2).Width = 30;
        worksheet.Column(3).Width = 20;

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    // ==========================================
    // 2. الاستيراد (مباشر وسريع: Item Name -> Item ID)
    // ==========================================
    public async Task<ImportResultDto> ImportAssetsFromExcelAsync(Stream fileStream, string createdByUserId)
    {
        var result = new ImportResultDto();
        var assetsToAdd = new List<Asset>();

        // 1. Caching: تحميل الأصناف (Items) في القاموس
        var allItems = await _context.Items.ToListAsync();
        var itemsDict = new Dictionary<string, Item>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in allItems)
        {
            // بنستخدم Trim عشان لو الإكسيل فيه مسافات زيادة
            if (!itemsDict.ContainsKey(item.Name.Trim()))
                itemsDict.Add(item.Name.Trim(), item);
        }

        // 2. تحميل المخازن
        var allWarehouses = await _context.Warehouses.ToListAsync();
        var warehousesDict = new Dictionary<string, Warehouse>(StringComparer.OrdinalIgnoreCase);
        foreach (var wh in allWarehouses)
        {
            if (!warehousesDict.ContainsKey(wh.Name.Trim()))
                warehousesDict.Add(wh.Name.Trim(), wh);
        }

        // 3. الباركودات لمنع التكرار
        var existingBarcodes = (await _context.Assets.Select(a => a.Barcode).ToListAsync())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        using (var workbook = new XLWorkbook(fileStream))
        {
            var worksheet = workbook.Worksheet(1);
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

            foreach (var row in rows)
            {
                try
                {
                    string itemName = row.Cell(1).GetValue<string>()?.Trim() ?? "";
                    string warehouseName = row.Cell(2).GetValue<string>()?.Trim() ?? "";
                    string barcode = row.Cell(3).GetValue<string>()?.Trim() ?? "";
                    string serialNumber = row.Cell(4).GetValue<string>()?.Trim() ?? "";

                    // Validation
                    if (string.IsNullOrWhiteSpace(itemName) || string.IsNullOrWhiteSpace(barcode))
                    {
                        result.Errors.Add($"سطر {row.RowNumber()}: بيانات ناقصة.");
                        continue;
                    }

                    // البحث عن الصنف (Item)
                    if (!itemsDict.TryGetValue(itemName, out var itemObj))
                    {
                        result.Errors.Add($"سطر {row.RowNumber()}: الصنف '{itemName}' غير موجود (تأكد أنك اخترته من القائمة).");
                        continue;
                    }

                    // البحث عن المخزن
                    if (!warehousesDict.TryGetValue(warehouseName, out var warehouseObj))
                    {
                        result.Errors.Add($"سطر {row.RowNumber()}: المخزن '{warehouseName}' غير موجود.");
                        continue;
                    }

                    // تكرار الباركود
                    if (existingBarcodes.Contains(barcode) || assetsToAdd.Any(a => a.Barcode.Equals(barcode, StringComparison.OrdinalIgnoreCase)))
                    {
                        result.Errors.Add($"سطر {row.RowNumber()}: الباركود '{barcode}' مكرر.");
                        continue;
                    }

                    // إنشاء الأصل
                    var asset = new Asset
                    {
                        Id = Guid.NewGuid(),
                        ItemId = itemObj.Id,          // ربطنا بالصنف المحدد
                        WarehouseId = warehouseObj.Id, // ربطنا بالمخزن المحدد
                        Barcode = barcode.ToUpper(),
                        SerialNumber = serialNumber,
                        Status = AssetStatus.Available,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = createdByUserId
                    };

                    assetsToAdd.Add(asset);
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"سطر {row.RowNumber()}: خطأ - {ex.Message}");
                }
            }
        }

        if (assetsToAdd.Any())
        {
            await _context.Assets.AddRangeAsync(assetsToAdd);
            await _context.SaveChangesAsync();
            result.SuccessCount = assetsToAdd.Count;
        }

        return result;
    }
}