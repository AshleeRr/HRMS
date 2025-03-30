using HRMS.Application.DTOs.Reservation_2023_0731;
using HRMS.Application.Interfaces.Reservation_2023_0731;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.InfraestructureInterfaces.Logging;
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
        private readonly IValidator<ReservationAddDTO> _validatorAdd;
        private readonly ILoggingServices _loggingServices;

        public ReservationsController(IReservationRepository reservationRepository, ILoggingServices logger
            , IValidator<ReservationAddDTO> validatorAdd, IReservationService reservationService)
        {
            _reservationServices = reservationService;
            _validatorAdd = validatorAdd;
            _reservationRepository = reservationRepository;
            _loggingServices = logger;
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
                var res = await _reservationServices.GetById(id);
                if(!res.IsSuccess)
                {
                    return BadRequest(res.Message);
                }
                return Ok(res.Data as ReservationDTO);
            }
            else
            {
                return BadRequest("0 no es un id valido");
            }

        }

        [HttpPost("CreateReservation")]
        public async Task<IActionResult> Post([FromBody] ReservationAddDTO reserv)
        {
            
            var validRes = _validatorAdd.Validate(reserv);
            if (!validRes.IsSuccess)
            {
                return BadRequest("Errores: " + validRes.Message);
            }
            
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
            if(dto.ID == 0)
            {
                return BadRequest("El ID no puede ser cero");
            }
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
            if(clientID == 0)
            {
                return BadRequest("El ID del cliente no puede ser cero");
            }
            var res = await _reservationRepository.GetReservationsByClientId(clientID);
            if (res.IsSuccess)
            {
                return Ok(res.Data);
            }
            return BadRequest(res.Message);
        }

        [HttpPut("ConfirmReservation/{reservationId}")]
        public async Task<IActionResult> ConfirmReservation(ReservationConfirmDTO dto)
        {
            if(dto.UserID == 0)
            {
                return BadRequest("El ID del usuario no puede ser cero");
            }
            if (dto.ReservationId == 0)
            {
                return BadRequest("El ID de la reservación no puede ser cero");
            }
            var res = await _reservationServices.ConfirmReservation(dto);
            if (res.IsSuccess)
            {
                return Ok(res.Message);
            }
            return BadRequest(res.Message);
        }

        [HttpPut("CancelReservation/{reservationId}")]
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



    }
}
