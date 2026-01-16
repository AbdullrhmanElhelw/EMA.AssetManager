namespace EMA.AssetManager.Services.Dtos.Items;

public class UpdateItemDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public int Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
}