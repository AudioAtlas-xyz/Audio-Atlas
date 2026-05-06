namespace AudioAtlasApplication.Repositories;
using AudioAtlasDomain.MusicMetadata;
using System.Collections.Generic;

public interface IInstrumentRepository
{
    public ICollection<Instrument> getAllInstruments();
    public Task<ICollection<Instrument>> getInstrumentsByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default);
}