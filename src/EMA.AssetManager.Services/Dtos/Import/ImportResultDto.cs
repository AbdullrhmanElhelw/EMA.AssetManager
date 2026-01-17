namespace EMA.AssetManager.Services.Dtos.Import;

public class ImportResultDto
{
    public int SuccessCount { get; set; }
    public List<string> Errors { get; set; } = new();
}