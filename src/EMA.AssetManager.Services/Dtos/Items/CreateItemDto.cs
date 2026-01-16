namespace EMA.AssetManager.Services.Dtos.Items;

public class CreateItemDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid CategoryId { get; set; } // هنا بنختار الفئة من القائمة
    public int Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
}