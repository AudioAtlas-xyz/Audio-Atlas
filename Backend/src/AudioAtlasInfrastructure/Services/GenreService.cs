using AudioAtlas.Domain.Geography;
using AudioAtlasApplication.Repositories;
using AudioAtlasApplication.Services;
using AudioAtlasDomain.Genres;
using AudioAtlasDomain.Geography;
using AudioAtlasDomain.MusicMetadata;

namespace AudioAtlasInfrastructure.Services;

public class GenreService : IGenreService
{
    private readonly IGenreRepository _genreRepository;

    public GenreService(IGenreRepository genreRepository)
    {
        _genreRepository = genreRepository;
    }

    public ICollection<GenreDTO> GetAllGenres()
    {
        return _genreRepository.getAllGenres().Select(g => CreateDTO(g)).ToList();
    }

    public GenreDTO? GetGenre(Guid id)
    {
        Genre genre = _genreRepository.getGenre(id);

        if(genre != null)
        {
            return CreateDTO(genre);
        }

        return null;
        
    }

    public async Task<ICollection<GenreDTO>> SearchForGenres(string keyword)
    {
        List<GenreDTO> genres = (await _genreRepository.SearchForGenres(keyword)).Select(g => CreateDTO(g)).ToList();

        genres.Sort();

        return genres;
    }


    public GenreDTO CreateDTO(Genre genre)
    {
        return new GenreDTO
        {
            Id = genre.Id,
            AuthorId = genre.AuthorId,
            Name = genre.Name,
            Description = genre.Description,
            Summary = genre.Summary,
            StartYear = genre.StartYear,
            PlaylistLink = genre.PlaylistLink,
            SensitiveDescription = genre.SensitiveDescription,
            IsSensitive = genre.IsSensitive,
            Aliases = genre.Aliases.Select(alias => new GenreAliasDTO
            {
                Alias = alias.Alias
            }).ToList(),
            Sources = genre.Sources.Select(source => new GenreSourceDTO
            {
                SourceLink = source.SourceLink
            }).ToList(),
            Countries = genre.Countries.Select(country => new CountryDTO
            {
                Id = country.Id,
                Name = country.Name,
                Description = country.Description,
                Region = country.Region,
                Continent = country.Continent,
                IsoCode = country.isoCode
            }).ToList(),
            Instruments = genre.Instruments.Select(instrument => new InstrumentDTO
            {
                Id = instrument.Id,
                Type = instrument.Type,
                Description = instrument.Description
            }).ToList(),
            SubGenres = genre.SubGenres.Select(subGenre => new GenreDTO
            {
                Id = subGenre.Id,
                Name = subGenre.Name,
                Countries = subGenre.Countries.Select(country => new CountryDTO
                {
                    Id = country.Id,
                    Name = country.Name,
                }).ToList()
            }).ToList(),
            ParentGenres = genre.ParentGenres.Select(parentGenre => new GenreDTO
            {
                Id = parentGenre.Id,
                Name = parentGenre.Name,
                Countries = parentGenre.Countries.Select(country => new CountryDTO
                {
                    Id = country.Id,
                    Name = country.Name,
                }).ToList()
            }).ToList(),
            SimilarGenres = genre.SimilarGenres.Select(similarGenre => new GenreDTO
            {
                Id = similarGenre.Id,
                Name = similarGenre.Name,
                Countries = similarGenre.Countries.Select(country => new CountryDTO
                {
                    Id = country.Id,
                    Name = country.Name,
                }).ToList()
            }).ToList(),
            Contributors = genre.Author != null && !genre.Author.IsSystemUser
                ? new List<ContributorSummaryDTO>
                {
                    new ContributorSummaryDTO
                    {
                        id = genre.Author.Id.ToString(),
                        username = genre.Author.UserName ?? genre.Author.Id.ToString(),
                        genreCount = _genreRepository.getGenresByAuthorId(genre.Author.Id).Count
                    }
                }
                : new List<ContributorSummaryDTO>()
        };
    }
}
