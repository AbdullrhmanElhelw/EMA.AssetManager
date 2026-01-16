namespace EMA.AssetManager.Services.Dtos.Categories.Create;

public class UpdateCategoryDto
{
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; }
}