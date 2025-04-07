using HRMS.WebApi.Models;
using HRMS.WebApi.Models.Reservation_2023_0731;
using Microsoft.AspNetCore.Mvc;
using WebApi.ServicesInterfaces.Reservation;

namespace HRMS.Web.Controllers.ReservationControllers
{
    public class ReservationController : Controller
    {
        // GET: ReservationController

        private readonly IReservationRepository _reservationService;

        public ReservationController(IReservationRepository reservationService)
        {
            _reservationService = reservationService;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<ReservationDTO> reservationList;
            var res = await _reservationService.GetAll();
            if (res.IsSuccess)
            {
                reservationList = res.Data as IEnumerable<ReservationDTO>; 
            }
            else
            {
                string errors = res.Message;
                ViewBag.Message = "Error al cargar la reserva" + errors;
                return View();
            }
            return View(reservationList);
        }

        // GET: ReservationController/Details/5
        public async Task<IActionResult> Details(int ReservationId)
        {
            ReservationDTO dto = null;
            var res = await _reservationService.GetById(ReservationId);
            if (res.IsSuccess)
            {
                dto = res.Data as ReservationDTO;

            }
            else
            {
                string errors = res.Message;
                ViewBag.Message = "Error al cargar la reserva" + errors;
                return View();
            }
            return View(dto);
        }

        // GET: ReservationController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ReservationController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReservationAddDTO reserv)
        {
            try
            {

                var res =  await _reservationService.Create(reserv);
                if (res.IsSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    string errors = res.Message;
                    ViewBag.Message = "Error al cargar la reserva" + errors;
                    return RedirectToAction(nameof(Index));
                }

            }
            catch
            {
                return View();
            }
        }

        // GET: ReservationController/Edit/5
        public async Task<IActionResult> Edit(int ReservationId)
        {
            ReservationUpdateDTO updateDTO = null;
            var res = await _reservationService.GetById(ReservationId);
            if (res.IsSuccess)
            {
                var reservationDTO = res.Data as ReservationDTO;
                updateDTO = new ReservationUpdateDTO
                {
                    UserID = reservationDTO.UserID,
                    ID = reservationDTO.ReservationId,
                    In = reservationDTO.EntryDate.Value,
                    Out = reservationDTO.DepartureDate.Value,
                    Observations = reservationDTO.Observation,
                    AbonoPenalidad = reservationDTO.PenaltyCost.Value,
                    ChangeTime = DateTime.Now
                };
            }
            else
            {
                string errors = res.Message;
                ViewBag.Message = "Error al cargar la reserva" + errors;
                return View();
            }
            return View(updateDTO);
        }

        // POST: ReservationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ReservationUpdateDTO updateDTO)
        {
            try
            {
                var response = await _reservationService.Update(updateDTO);
                if (response.IsSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    string errors = response.Message;
                    ViewBag.Message = "Error al cargar la reserva" + errors;
                    return RedirectToAction(nameof(Index));
                }
            }
            catch
            {
                return View();
            }
        }



    }
}
