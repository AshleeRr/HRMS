using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class HabitacionController : ControllerBase
    {
        private readonly IHabitacionRepository _habitacionRepository;
        private readonly ILogger<HabitacionController> _logger;
        // GET: api/<HabitacionController>
        
        public HabitacionController(IHabitacionRepository habitacionRepository , ILogger<HabitacionController> logger)
        {
            _habitacionRepository = habitacionRepository;
            _logger = logger;
        }
        [HttpGet("GetHabitaciones")]
        public async Task<ActionResult> Get()
        {
            var habitaciones = await _habitacionRepository.GetAllAsync();
            return Ok(habitaciones);
        }
    
        // GET api/<HabitacionController>/5
        [HttpGet("GetHabitacionById")]
        public async Task<ActionResult>Get(int id)
        {
            var habitacion = await _habitacionRepository.GetEntityByIdAsync(id);
            return Ok(habitacion);
        }

        // POST api/<HabitacionController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<HabitacionController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<HabitacionController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
