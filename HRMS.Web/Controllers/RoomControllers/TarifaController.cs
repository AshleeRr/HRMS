using HRMS.Application.DTOs.RoomManagementDto.TarifaDtos;
using HRMS.Application.Interfaces.RoomManagementService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Web.Controllers
{
    public class TarifaController : Controller
    {
        private readonly ITarifaService tarifaService;
        
        public TarifaController(ITarifaService tarifaService)
        {
            this.tarifaService = tarifaService;
        }
        // GET: Tarifa
        public async Task <IActionResult> Index()
        {
            var result = await tarifaService.GetAll();
            if (result.IsSuccess)
            {
                List<TarifaDto> tarifaList = (List<TarifaDto>)result.Data;
                return View(tarifaList);
            }
            return View();
        }

        // GET: Tarifa/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Tarifa/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tarifa/Create
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

        // GET: Tarifa/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Tarifa/Edit/5
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

        // GET: Tarifa/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Tarifa/Delete/5
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
