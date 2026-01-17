using EMA.AssetManager.Services.Dtos.Search;

public interface ISearchService
{
    Task<List<SearchResultDto>> SearchAsync(string query);
}