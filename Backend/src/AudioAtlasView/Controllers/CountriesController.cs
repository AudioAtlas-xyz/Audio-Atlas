using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AudioAtlasView.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        // GET: api/countries
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
        
        // GET: api/countries/{id string}/genres
        [HttpGet("{id}/genres")]
        public string Get(int id)
        {
            return "value";
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
