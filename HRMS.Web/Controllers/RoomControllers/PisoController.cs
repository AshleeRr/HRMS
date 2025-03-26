using HRMS.Application.DTOs.RoomManagementDto.PisoDtos;
using HRMS.Application.Interfaces.RoomManagementService;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Web.Controllers.RoomControllers
{
    public class PisoController : Controller
    {
        private readonly IPisoService pisoService;

        public PisoController(IPisoService pisoService)
        {
            this.pisoService = pisoService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await pisoService.GetAll();
            if (result.IsSuccess)
            {
                List<PisoDto> pisoList = (List<PisoDto>)result.Data;
                return View(pisoList);
            }

            return View("Index");
        }

        // GET: PisoController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }


        // POST: PisoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePisoDto dto)
        {
            try
            {
                var result = await pisoService.Save(dto);
                if (result.IsSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }

                return View("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: PisoController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var result = await pisoService.GetById(id);
                if (!result.IsSuccess)
                {
                    TempData["ErrorMessage"] = result.Message ?? "El piso no fue encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                var piso = result.Data as PisoDto;
                if (piso == null)
                {
                    TempData["ErrorMessage"] = "Error al obtener los pisos de la habitacion.";
                    return RedirectToAction(nameof(Index));
                }

                var updateDto = new UpdatePisoDto()
                {
                    IdPiso = piso.IdPiso,
                    Descripcion = piso.Descripcion
                };

                return View(updateDto);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar los datos: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: PisoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdatePisoDto update)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(update);
                }

                var result = await pisoService.Update(update);
                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Piso actualizado correctamente.";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", result.Message ?? "No se pudo actualizar el piso.");
                return View(update);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar: " + ex.Message);
                return View(update);
            }
        }

        // POST: PisoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var delete = new DeletePisoDto() { IdPiso = id };
                var result = await pisoService.Remove(delete);

                if (result != null && result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Piso eliminado correctamente.";
                    return Json(new { success = true, message = "Piso eliminado correctamente." });
                }
                else
                {
                    string errorMessage = result?.Message ?? "No se pudo eliminar el piso.";
                    return Json(new { success = false, message = errorMessage });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al eliminar: " + ex.Message });
            }
        }
    }
}
