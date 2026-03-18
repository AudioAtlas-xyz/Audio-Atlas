using AudioAtlasDomain.Geography;
using AudioAtlasInfrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AudioAtlasInfrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AudioAtlasView.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private CountryRepository _countryRepository = new CountryRepository(new AppDbContext(new DbContextOptions<AppDbContext>()));
        // GET: api/countries 
        [HttpGet]
        public Dictionary<Country,int> Get()
        {
            return _countryRepository.getGenreCounts();
        }
        
        // GET: api/countries/{id string}/genres
        [HttpGet("{id}/genres")]
        public string Get(int id)
        {
            return "value";
            //getGenres(genre)
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
