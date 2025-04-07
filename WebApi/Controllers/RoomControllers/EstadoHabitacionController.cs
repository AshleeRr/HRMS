using Microsoft.AspNetCore.Mvc;
using WebApi.Interfaces;
using WebApi.Interfaces.RoomInterface;
using WebApi.Models.RoomModels;

namespace WebApi.Controllers.RoomControllers
{
    public class EstadoHabitacionController : Controller
    {
        private readonly IEstadoHabitacionRepository _estadoHabitacionRepository;

        public EstadoHabitacionController(IEstadoHabitacionRepository estadoHabitacionRepository)
        {
            _estadoHabitacionRepository = estadoHabitacionRepository;
        }
        
        // GET: EstadoHabitacionController
        public async Task<IActionResult> Index()
        {
            List<EstadoHabitacionModel> estados = new List<EstadoHabitacionModel>();
            try
            {
                estados = (await _estadoHabitacionRepository.GetAllAsync()).ToList();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
            }

            return View(estados);
        }

        // GET: EstadoHabitacionController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var estado = await _estadoHabitacionRepository.GetByIdAsync(id);
                return View(estado);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: EstadoHabitacionController/Create
        public ActionResult Create()
        {
            return View(new EstadoHabitacionModel());
        }

        // POST: EstadoHabitacionController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EstadoHabitacionModel estado)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _estadoHabitacionRepository.CreateAsync(estado);
                    
                    if (result.IsSuccess)
                    {
                        TempData["Success"] = result.Message ?? "Estado creado correctamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    
                    TempData["Error"] = result.Message ?? "Error al crear la estado de habitacion.";
                }
                
                return View(estado);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return View(estado);
            }
        }

        // GET: EstadoHabitacionController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var estado = await _estadoHabitacionRepository.GetByIdAsync(id);
                
                if (estado != null && estado.IdEstadoHabitacion != id)
                {
                    estado.IdEstadoHabitacion = id;
                }
                return View(estado);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: EstadoHabitacionController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EstadoHabitacionModel estado)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (estado.IdEstadoHabitacion != id)
                    {
                        estado.IdEstadoHabitacion = id;
                    }
                    var result = await _estadoHabitacionRepository.UpdateAsync(id, estado);
            
                    if (result.IsSuccess)
                    {
                        TempData["Success"] = result.Message ?? "Estado habitacion actualizado correctamente.";
                        return RedirectToAction(nameof(Index));
                    }
            
                    TempData["Error"] = result.Message ?? "Error al actualizar el estado.";
                }
        
                return View(estado);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return View(estado);
            }
        }

        // GET: EstadoHabitacionController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var estado = await _estadoHabitacionRepository.GetByIdAsync(id);
                return View(estado);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
            
        }

        // POST: EstadoHabitacionController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                var result = await _estadoHabitacionRepository.DeleteAsync(id);
                
                if (result.IsSuccess)
                {
                    TempData["Success"] = result.Message ?? "Estado eliminado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                
                TempData["Error"] = result.Message ?? "Error al eliminar el estado.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
            }
            
            return RedirectToAction(nameof(Index));
        }

        // GET: EstadoHabitacionController/GetByDescripcion
        public async Task<ActionResult> GetByDescripcion(string descripcion)
        {
            try
            {
                if (string.IsNullOrEmpty(descripcion))
                {
                    TempData["Error"] = "La descripción no puede ser nula o vacía.";
                    return RedirectToAction(nameof(Index));
                }

                var piso = await _estadoHabitacionRepository.GetByDescripcionAsync(descripcion);
                
                if (piso == null)
                {
                    TempData["Error"] = $"No se encontró un Estados con la descripción: {descripcion}";
                    return RedirectToAction(nameof(Index));
                }
                
                return View("Details", piso);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}