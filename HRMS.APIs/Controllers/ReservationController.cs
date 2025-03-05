using HRMS.Domain.Base;
using HRMS.Domain.Entities.Reservation;
using HRMS.Domain.Repository;
using Microsoft.AspNetCore.Mvc;


namespace HRMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly ILogger<ReservationsController> _logger;

        public ReservationsController(IReservationRepository reservationRepository, ILogger<ReservationsController> logger)
        {
            _reservationRepository = reservationRepository;
            _logger = logger;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> Get()
        {
            var res = await _reservationRepository.GetAllAsync();
            return Ok(res);
        }

        [HttpGet("GetByID/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (id != 0)
            {
                var res = await _reservationRepository.GetEntityByIdAsync(id);
                return Ok(res);
            }
            else
            {
                return BadRequest("0 no es un id valido");
            }

        }

        [HttpPost("CreateReservation")]
        public async Task<IActionResult> Post([FromBody] Reservation reserv)
        {

            var validRes = _validSave(reserv);
            if (!validRes.IsSuccess)
            {
                return BadRequest("Errores: " + validRes.Message);
            }
            var res = await _reservationRepository.SaveEntityAsync(reserv);
            if (res.IsSuccess)
            {
                return Created();
            }
            return BadRequest(res.Message);
        }

        [HttpPut("UpdateReservation/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Reservation reser)
        {
            var validRes = _validSave(reser);
            if (!validRes.IsSuccess)
            {
                return BadRequest("Errores: " + validRes.Message);
            }
            var reservOrigin = await _reservationRepository.GetEntityByIdAsync(id);
            if (reservOrigin == null)
                return BadRequest("La reservación a Actualizar es Inexistente");

            reservOrigin.IdCliente = reser.IdCliente;
            reservOrigin.IdHabitacion = reser.IdHabitacion;
            reservOrigin.Adelanto = reser.Adelanto;
            reservOrigin.CostoPenalidad = reser.CostoPenalidad;
            reservOrigin.TotalPagado = reser.TotalPagado;
            reservOrigin.FechaEntrada = reser.FechaEntrada;
            reservOrigin.FechaSalida = reser.FechaSalida;
            reservOrigin.FechaSalidaConfirmacion = reser.FechaSalidaConfirmacion;
            reservOrigin.PrecioInicial = reser.PrecioInicial;
            reservOrigin.PrecioRestante = reser.PrecioRestante;

            var res = await _reservationRepository.UpdateEntityAsync(reser);
            if (res.IsSuccess)
            {
               return NoContent();
            }
            return BadRequest(res.Message);
        }

        [HttpGet("GetReservationsByClientId/{clientID}")]
        public async Task<IActionResult> GetReservationsByClientId(int clientID)
        {
            var res = await _reservationRepository.GetReservationsByClientId(clientID);
            if (res.IsSuccess)
            {
                return Ok(res.Data);
            }
            return BadRequest(res.Message);
        }



        private OperationResult _validSave(Reservation r)
        {
            OperationResult operationResult = new OperationResult();
            List<string> errors = new List<string>(); 
            if(r.IdCliente == 0)
            {
                errors.Add("El ID del cliente no puede ser cero");
            }
            if (r.IdHabitacion == 0)
            {
                errors.Add("El ID de la habitación no puede ser cero");
            }
            if(r.PrecioInicial == 0)
            {
                errors.Add("El Precio Inicial la habitación no puede ser cero");
            }
            if(r.TotalPagado == 0)
            {
                errors.Add("El total pagado no puede ser cero");
            }
            if(r.FechaEntrada == null)
            {
                errors.Add("La fecha de entrada no puede ser nula");
            }
            if(r.FechaSalida == null)
            {
                errors.Add("La fecha de salida no puede ser nula");
            }


            if (errors.Count > 0)
            {
                operationResult.IsSuccess = false;
                operationResult.Message =  string.Join(Environment.NewLine, errors);
            }
            return operationResult;
        }

    }
}
