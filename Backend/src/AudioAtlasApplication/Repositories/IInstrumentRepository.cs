namespace AudioAtlasApplication.Repositories;
using AudioAtlasDomain.MusicMetadata;
using System.Collections.Generic;

public interface IInstrumentRepository
{
    public ICollection<Instrument> getAllInstruments();
}