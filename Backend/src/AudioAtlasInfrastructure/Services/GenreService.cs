using AudioAtlasApplication.Repositories;
using AudioAtlasApplication.Services;
using AudioAtlasDomain.Geography;

namespace AudioAtlasInfrastructure.Services;
using AudioAtlasDomain.Genres;

public class GenreService : IGenreService
{
    IGenreRepository _genreRepository;
    
    public GenreService(IGenreRepository genreRepository)
    {
        _genreRepository = genreRepository;
    }

    /// <summary>
    /// Takes a genre and converts it into a GenreDTO
    /// </summary>
    /// <param name="id"> The ID corresponding to a genre </param>
    /// <returns> A GenreDTO </returns>
    public GenreDTO getGenre(Guid id)
    {
        Genre genre = _genreRepository.getGenre(id);

        if (genre == null) return null;

        return new GenreDTO()
        {
            Id = genre.Id,
            AuthorId = genre.AuthorId,
            Name = genre.Name,
            Description = genre.Description,
            SensitiveDescription = genre.SensitiveDescription,
            IsSensitive = genre.IsSensitive,
            Aliases = genre.Aliases,
            
            // Only ID of countries
            Countries = genre.Countries.Select(c => new CountryDTO
            {
                Id =  c.Id,
                Name = c.Name,
                Description =  c.Description,
            }).ToList(),

            // Gather only IDs from lists
            SubGenres = genre.SubGenres.Select(p => new GenreDTO 
                {
                    Id = p.Id,
                    Name = p.Name 
                }).ToList(),
            ParentGenres = genre.ParentGenres.Select(p => new GenreDTO
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList(),
            SimilarGenres = genre.SimilarGenres.Select(p => new GenreDTO
            {
                Id = p.Id,
                Name = p.Name
            }).ToList(),            
        };

    }
    
}