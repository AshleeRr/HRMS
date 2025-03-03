using HRMS.Application.DTOs.Reservation_2023_0731;
using HRMS.Application.Interfaces.Reservation_2023_0731;
using HRMS.Domain.Base;
using HRMS.Domain.Entities.Reservations;
using HRMS.Domain.Repository;
using Microsoft.AspNetCore.Mvc;


namespace HRMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IReservationService _reservationServices;
        private readonly ILogger<ReservationsController> _logger;

        public ReservationsController(IReservationRepository reservationRepository, ILogger<ReservationsController> logger, IReservationService reservationService)
        {
            _reservationServices = reservationService;
            _reservationRepository = reservationRepository;
            _logger = logger;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> Get()
        {
            var res = await _reservationServices.GetAll();
            if(!res.IsSuccess)
            {
                return BadRequest(res.Message);
            }
            IEnumerable<ReservationDTO> dtos = res.Data as IEnumerable<ReservationDTO>;
            return Ok(res.Data);
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
        public async Task<IActionResult> Post([FromBody] ReservationAddDTO reserv)
        {
            /*
            var validRes = _validSave(reserv);
            if (!validRes.IsSuccess)
            {
                return BadRequest("Errores: " + validRes.Message);
            }
            */
            var res = await _reservationServices.Save(reserv);
            if (res.IsSuccess)
            {
                return Created();
            }
            return BadRequest(res.Message);
        }

        [HttpPut("UpdateReservation")]
        public async Task<IActionResult> Put(ReservationUpdateDTO dto)
        {
            var res = await _reservationServices.Update(dto);
            if (res.IsSuccess)
            {
                return Ok(res.Data);
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

        [HttpPatch("ConfirmReservation/{reservationId}")]
        public async Task<IActionResult> ConfirmReservation(ReservationConfirmDTO dto)
        {
            var res = await _reservationServices.ConfirmReservation(dto);
            if (res.IsSuccess)
            {
                return Ok(res.Message);
            }
            return BadRequest(res.Message);
        }

        [HttpPatch("CancelReservation/{reservationId}")]
        public async Task<IActionResult> CancelReservation(int reservationId)
        {
            var res = await _reservationServices.CancelReservation(reservationId);
            if (res.IsSuccess)
            {
                return Ok(res.Message);
            }
            return BadRequest(res.Message);
        }

        [HttpDelete("RemoveReservation")]
        public async Task<IActionResult> Remove(ReservationRemoveDTO dTO)
        {
            var res = await _reservationServices.Remove(dTO);
            if (res.IsSuccess)
            {
                return Ok(res.Message);
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
