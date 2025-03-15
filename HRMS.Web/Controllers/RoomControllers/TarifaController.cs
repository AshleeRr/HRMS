using HRMS.Application.DTOs.RoomManagementDto.TarifaDtos;
using HRMS.Application.Interfaces.RoomManagementService;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Web.Controllers.RoomControllers
{
    public class TarifaController : Controller
    {
        private readonly ITarifaService tarifaService;

        public TarifaController(ITarifaService tarifaService)
        {
            this.tarifaService = tarifaService;
        }

        // GET: Tarifa
        public async Task<IActionResult> Index()
        {
            var result = await tarifaService.GetAll();
            if (result.IsSuccess && result.Data != null)
            {
                List<TarifaDto> tarifaList = (List<TarifaDto>)result.Data;
                return View(tarifaList);
            }

            TempData["ErrorMessage"] = result.Message ?? "No se pudieron cargar las tarifas.";
            return View(new List<TarifaDto>());
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
        public async Task<IActionResult> Create(CreateTarifaDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(dto);
                }

                var result = await tarifaService.Save(dto);
                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Tarifa creada correctamente.";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", result.Message ?? "No se pudo crear la tarifa.");
                return View(dto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear: " + ex.Message);
                return View(dto);
            }
        }

        // GET: Tarifa/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var result = await tarifaService.GetById(id);
                if (!result.IsSuccess)
                {
                    TempData["ErrorMessage"] = result.Message ?? "La tarifa no fue encontrada.";
                    return RedirectToAction(nameof(Index));
                }

                var tarifa = result.Data as TarifaDto;
                if (tarifa == null)
                {
                    TempData["ErrorMessage"] = "Error al obtener las tarifas.";
                    return RedirectToAction(nameof(Index));
                }

                var updateDto = new UpdateTarifaDto()
                {
                    IdTarifa = tarifa.IdTarifa,
                    Descripcion = tarifa.Descripcion,
                    PrecioPorNoche = tarifa.PrecioPorNoche,
                    Descuento = tarifa.Descuento,
                    FechaInicio = tarifa.FechaInicio,
                    FechaFin = tarifa.FechaFin,
                    IdCategoria = tarifa.IdCategoria
                };

                return View(updateDto);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar los datos: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Tarifa/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateTarifaDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(dto);
                }

                var result = await tarifaService.Update(dto);
                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Tarifa actualizada correctamente.";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", result.Message ?? "No se pudo actualizar la tarifa.");
                return View(dto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar: " + ex.Message);
                return View(dto);
            }
        }
    }
}

