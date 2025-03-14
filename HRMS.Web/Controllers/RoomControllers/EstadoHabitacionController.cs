using HRMS.Application.DTOs.RoomManagementDto.EstadoHabitacionDtos;
using HRMS.Application.Interfaces.RoomManagementService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Web.Controllers.RoomControllers
{
    public class EstadoHabitacionController : Controller
    {
        private readonly IEstadoHabitacionService estadoHabitacionService;

        public EstadoHabitacionController(IEstadoHabitacionService estadoHabitacionService)
        {
            this.estadoHabitacionService = estadoHabitacionService;
        }

        // GET: EstadoHabitacionController
        public async Task <IActionResult> Index()
        {
            var result = await estadoHabitacionService.GetAll();
            if (result.IsSuccess)
            {
                List<EstadoHabitacionDto> estadoHabitacionList = (List<EstadoHabitacionDto>)result.Data;
                return View(estadoHabitacionList);
            }
            return View();
        }

        // GET: EstadoHabitacionController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: EstadoHabitacionController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EstadoHabitacionController/Create
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

        // GET: EstadoHabitacionController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: EstadoHabitacionController/Edit/5
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

        // GET: EstadoHabitacionController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EstadoHabitacionController/Delete/5
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
