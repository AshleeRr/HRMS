using HRMS.Application.DTOs.RoomManagementDto.HabitacionDtos;
using HRMS.Application.Interfaces.RoomManagementService;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Web.Controllers.RoomControllers
{
    public class RoomController : Controller
    {
        private readonly IHabitacionService _habitacionService;

        public RoomController(IHabitacionService habitacionService)
        {
            this._habitacionService = habitacionService;
        }

        // GET: RoomController
        public async Task<IActionResult> Index()
        {
            var result = await _habitacionService.GetAll();
            if (result.IsSuccess)
            {
                List<HabitacionDto> habitacionList = (List<HabitacionDto>)result.Data;
                return View(habitacionList);
            }

            return View("Index");
        }

        // GET: RoomController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RoomController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateHabitacionDTo dTo)
        {
            try
            {
                var result = await _habitacionService.Save(dTo);
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

        // GET: RoomController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var result = await _habitacionService.GetById(id);
                if (!result.IsSuccess)
                {
                    TempData["ErrorMessage"] = result.Message ?? "La habitación no fue encontrada.";
                    return RedirectToAction(nameof(Index));
                }

                var habitacion = result.Data as HabitacionDto;
                if (habitacion == null)
                {
                    TempData["ErrorMessage"] = "Error al obtener los datos de la habitación.";
                    return RedirectToAction(nameof(Index));
                }

                var updateDto = new UpdateHabitacionDto
                {
                    IdHabitacion = habitacion.IdHabitacion,
                    Numero = habitacion.Numero,
                    Detalle = habitacion.Detalle,
                    Precio = habitacion.Precio,
                    IdEstadoHabitacion = habitacion.IdEstadoHabitacion,
                    IdPiso = habitacion.IdPiso,
                    IdCategoria = habitacion.IdCategoria
                };

                return View(updateDto);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar los datos: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: RoomController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateHabitacionDto update)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(update);
                }

                var result = await _habitacionService.Update(update);
                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Habitación actualizada correctamente.";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", result.Message ?? "No se pudo actualizar la habitación.");
                return View(update);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar: " + ex.Message);
                return View(update);
            }
        }

        // POST: RoomController/Delete/5
        // POST: HabitacionController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var checkResult = await _habitacionService.GetById(id);
                if (!checkResult.IsSuccess)
                {
                    return Json(new { success = false, message = checkResult.Message ?? "No se encontró la habitación." });
                }
        
                var delete = new DeleteHabitacionDto() { IdHabitacion = id }; 
                var result = await _habitacionService.Remove(delete);
        
                if (result != null && result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Habitación eliminada correctamente.";
                    return Json(new { success = true, message = "Habitación eliminada correctamente." });
                }
                else
                {
                    string errorMessage = result?.Message ?? "No se pudo eliminar la habitación.";
                    return Json(new { success = false, message = errorMessage });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al eliminar: " + ex.Message });
            }
        }
        public async Task<IActionResult> InfoHabitaciones()
        {
            try
            {
                var resultado = await _habitacionService.GetInfoHabitacionesAsync();

                if (!resultado.IsSuccess)
                {
                    TempData["ErrorMessage"] = resultado.Message ?? "Error al obtener la información de las habitaciones";
                    return View(new List<HabitacionInfoDto>());
                }

                if (resultado.Data == null)
                {
                    TempData["WarningMessage"] = "No se encontró información de habitaciones";
                    return View(new List<HabitacionInfoDto>());
                }

                var habitacionesInfo = resultado.Data as List<HabitacionInfoDto>;
            
                if (habitacionesInfo == null)
                {
                    TempData["ErrorMessage"] = "Error al procesar los datos de habitaciones";
                    return View(new List<HabitacionInfoDto>());
                }

                return View(habitacionesInfo);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ocurrió un error: {ex.Message}";
                return View(new List<HabitacionInfoDto>());
            }
        }
    }
}
