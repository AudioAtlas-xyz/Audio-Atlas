using AudioAtlasApplication.Repositories;
using AudioAtlasApplication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AudioAtlasDomain.Genres;

namespace AudioAtlasView.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IGenreRepository _genreRepository;

        public GenresController(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }
        
        // GET: api/genres
        [HttpGet]
        public ICollection<Genre> Get()
        {
            return _genreRepository.getAllGenres();
        }
        
        // GET api/genres/{id}
        [HttpGet("{id}")]
        public Genre Get(Guid id)
        {
            return _genreRepository.getGenre(id);
        }

        // POST api/<GenresController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<GenresController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<GenresController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
