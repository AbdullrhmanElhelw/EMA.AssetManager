namespace EMA.AssetManager.Services.Dtos.Categories.Create;

public class CreateCategoryDto
{
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; } = true;
}
