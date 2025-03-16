using HRMS.Application.DTOs.RoomManagementDto.EstadoHabitacionDtos;
using HRMS.Application.Interfaces.RoomManagementService;
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
        public async Task <IActionResult> Create(CreateEstadoHabitacionDto dto)
        {
            try
            {
                var result = await estadoHabitacionService.Save(dto);
                if (result.IsSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }
                
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: EstadoHabitacionController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var result = await estadoHabitacionService.GetById(id);
                if (!result.IsSuccess)
                {
                    TempData["ErrorMessage"] = result.Message ?? "El Estado Habitacion no fue encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                var estadoHabitacion = result.Data as EstadoHabitacionDto;
                if (estadoHabitacion == null)
                {
                    TempData["ErrorMessage"] = "Error al obtener los estados de la habitacion.";
                    return RedirectToAction(nameof(Index));
                }

                var updateDto = new UpdateEstadoHabitacionDto()
                {
                    IdEstadoHabitacion = estadoHabitacion.IdEstadoHabitacion,
                    Descripcion = estadoHabitacion.Descripcion
                };

                return View(updateDto);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar los datos: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: EstadoHabitacionController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> Edit(UpdateEstadoHabitacionDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }

                var result = await estadoHabitacionService.Update(dto);
                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Estado de Habitacion actualizado correctamente.";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", result.Message ?? "No se pudo actualizar el estado.");
                return View(dto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar: " + ex.Message);
                return View(dto);
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
        public async Task <IActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                var delete = new DeleteEstadoHabitacionDto() { IdEstadoHabitacion = id };
                var result = await estadoHabitacionService.Remove(delete);

                if (result != null && result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Estado Habitacion eliminado correctamente.";
                    return Json(new { success = true, message = "Estado Habitacion eliminado correctamente." });
                }
                else
                {
                    string errorMessage = result?.Message ?? "No se pudo eliminar el Estado de habitacion.";
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
