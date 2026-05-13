using AudioAtlasApplication.Repositories;
using AudioAtlasInfrastructure.Database;
using AudioAtlasDomain.MusicMetadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace AudioAtlasInfrastructure.Repositories;

public class InstrumentRepository : IInstrumentRepository
{
    readonly AppDbContext _dbcontext;
    
    public InstrumentRepository(AppDbContext dbcontext)
    {
        _dbcontext = dbcontext;
    }
    public ICollection<Instrument> getAllInstruments()
    {
        return _dbcontext.Instruments.ToList();
    }

    public async Task<ICollection<Instrument>> getInstrumentsByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default)
    {
        return await _dbcontext.Instruments
            .Where(i => ids.Contains(i.Id))
            .ToListAsync(cancellationToken);
    }
}