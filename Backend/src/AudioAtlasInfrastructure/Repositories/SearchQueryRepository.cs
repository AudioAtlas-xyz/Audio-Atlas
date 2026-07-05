using AudioAtlasApplication.Repositories;
using AudioAtlasDomain.Search;
using AudioAtlasInfrastructure.Database;

namespace AudioAtlasInfrastructure.Repositories;

public class SearchQueryRepository : ISearchQueryRepository
{
    private readonly AppDbContext _dbContext;

    public SearchQueryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task logAsync(SearchQuery query, CancellationToken cancellationToken = default)
    {
        await _dbContext.SearchQueries.AddAsync(query, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
