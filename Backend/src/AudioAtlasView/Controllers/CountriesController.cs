using AudioAtlasDomain.Geography;
using AudioAtlasInfrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AudioAtlasInfrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using AudioAtlasApplication.Repositories;
using AudioAtlasApplication.Services;
using AudioAtlasDomain.Genres;

namespace AudioAtlasView.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly ICountryRepository _countryRepository;
        private readonly ICountryService _countryService;

        public CountriesController(ICountryRepository countryRepository, ICountryService countryService)
        {
            _countryRepository = countryRepository;
            _countryService = countryService;
        }
        
        // GET: api/countries 
        [HttpGet]
        public Dictionary<string,int> Get()
        {
            return _countryRepository.getGenreCounts();
        }
        
        // GET: api/countries/all
        [HttpGet("all")]
        public ICollection<Country> GetCountries()
        {
            return _countryRepository.getAllCountries();
        }
        
        // GET: api/countries/{id}/genres
        [HttpGet("{id}/genres")]
        public ICollection<Genre> Get(Guid id)
        {
            return _countryRepository.getGenres(id);
        }
        
        // GET: api/countries/{id}
        [HttpGet("{id}")]
        public CountryDTO Getter(Guid id)
        {
            return _countryService.getCountryById(id);
        }
        
     

        // POST api/<ViewController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ViewController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ViewController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
