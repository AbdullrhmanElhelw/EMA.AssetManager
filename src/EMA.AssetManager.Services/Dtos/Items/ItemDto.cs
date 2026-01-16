namespace EMA.AssetManager.Services.Dtos.Items;

public class ItemDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;

    // عشان نعرض اسم الفئة في الجدول بدل الـ ID
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}
