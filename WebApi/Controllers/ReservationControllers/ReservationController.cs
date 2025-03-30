using HRMS.WebApi.Models.Reservation_2023_0731;
using Microsoft.AspNetCore.Http;
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
                    ViewBag.Message = "Error al cargar las reservas";
                    return View();
                }
            }
            return View(reservationList);
        }

        // GET: ReservationController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ReservationController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ReservationController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ReservationController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ReservationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ReservationController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ReservationController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
