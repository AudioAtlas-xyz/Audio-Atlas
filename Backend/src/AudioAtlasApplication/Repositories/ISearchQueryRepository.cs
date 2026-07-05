using AudioAtlasDomain.Search;

namespace AudioAtlasApplication.Repositories;

public interface ISearchQueryRepository
{
    Task logAsync(SearchQuery query, CancellationToken cancellationToken = default);
}
