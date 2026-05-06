using AudioAtlasApplication.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AudioAtlasDomain.MusicMetadata;
using AudioAtlasInfrastructure.Repositories;

namespace AudioAtlasView.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstrumentsController : ControllerBase
    {
        private readonly IInstrumentRepository _instrumentRepository;
        
        public InstrumentsController(IInstrumentRepository instrumentRepository)
        {
            _instrumentRepository = instrumentRepository;
        }
        
        //GET: api/instruments
        [HttpGet]
        public ICollection<Instrument> Get()
        {
            return _instrumentRepository.getAllInstruments();
        }
        
        
        
    }
    
}

