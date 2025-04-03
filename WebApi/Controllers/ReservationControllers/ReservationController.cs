using HRMS.WebApi.Models;
using HRMS.WebApi.Models.Reservation_2023_0731;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Web.Controllers.ReservationControllers
{
    public class ReservationController : Controller
    {
        // GET: ReservationController
        public async Task<IActionResult> Index()
        {
            List<ReservationDTO> reservationList = new List<ReservationDTO>();
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7175/api/");
                var response = await client.GetAsync("Reservations/GetAll");
                if(response.IsSuccessStatusCode)
                {
                    reservationList = await response.Content.ReadFromJsonAsync<List<ReservationDTO>>();
                }
                else
                {
                    string errors = await response.Content.ReadAsStringAsync();
                    ViewBag.Message = "Error al cargar la reserva" + errors;
                    return View();
                }
            }
            return View(reservationList);
        }

        // GET: ReservationController/Details/5
        public async Task<IActionResult> Details(int ReservationId)
        {
            ReservationDTO dto = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7175/api/");
                var response = await client.GetAsync($"Reservations/GetByID/{ReservationId}");
                if (response.IsSuccessStatusCode)
                {
                    dto = await response.Content.ReadFromJsonAsync<ReservationDTO>();
                    
                }
                else
                {
                    string errors = await response.Content.ReadAsStringAsync();
                    ViewBag.Message = "Error al cargar la reserva" + errors;
                    return View();
                }
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
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7175/api/");
                    var response = await client.PostAsJsonAsync("Reservations/CreateReservation", reserv);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        string errors = await response.Content.ReadAsStringAsync();
                        ViewBag.Message = "Error al cargar la reserva" + errors;
                        return RedirectToAction(nameof(Index));
                    }
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
            using (var client =  new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7175/api/");
                var response = await client.GetAsync($"Reservations/GetByID/{ReservationId}");
                if (response.IsSuccessStatusCode)
                {
                    var reservationDTO = await response.Content.ReadFromJsonAsync<ReservationDTO>();
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
                    string errors = await response.Content.ReadAsStringAsync();
                    ViewBag.Message = "Error al cargar la reserva" + errors;
                    return View();
                }
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
                using(var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7175/api/");
                    var response = await client.PutAsJsonAsync("Reservations/UpdateReservation", updateDTO);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        string errors = await response.Content.ReadAsStringAsync();
                        ViewBag.Message = "Error al cargar la reserva" + errors;
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {
                return View();
            }
        }


}
    }
}
