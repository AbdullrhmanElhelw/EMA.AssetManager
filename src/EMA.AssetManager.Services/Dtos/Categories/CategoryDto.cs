namespace EMA.AssetManager.Services.Dtos.Categories;

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ItemsCount { get; set; }
}