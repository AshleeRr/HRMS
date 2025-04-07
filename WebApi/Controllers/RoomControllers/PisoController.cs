using Microsoft.AspNetCore.Mvc;
using WebApi.Interfaces;
using WebApi.Interfaces.RoomInterface;
using WebApi.Models.RoomModels;

namespace WebApi.Controllers.RoomControllers
{
    public class PisoController : Controller
    {
        private readonly IPisoRepository _pisoRepository;

        public PisoController(IPisoRepository pisoRepository)
        {
            _pisoRepository = pisoRepository;
        }

        // GET: PisoController
        public async Task<IActionResult> Index()
        {
            List<PisoModel> pisos = new List<PisoModel>();
            try
            {
                pisos = (await _pisoRepository.GetAllAsync()).ToList();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
            }

            return View(pisos);
        }

        // GET: PisoController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var piso = await _pisoRepository.GetByIdAsync(id);
                return View(piso);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: PisoController/Create
        public ActionResult Create()
        {
            return View(new PisoModel());
        }

        // POST: PisoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PisoModel piso)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _pisoRepository.CreateAsync(piso);
                    
                    if (result.IsSuccess)
                    {
                        TempData["Success"] = result.Message ?? "Piso creado correctamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    
                    TempData["Error"] = result.Message ?? "Error al crear la piso.";
                }
                
                return View(piso);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return View(piso);
            }
        }

        // GET: PisoController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var piso = await _pisoRepository.GetByIdAsync(id);
                
                if (piso != null && piso.IdPiso != id)
                {
                    piso.IdPiso = id;
                }
                return View(piso);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: PisoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PisoModel piso)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (piso.IdPiso != id)
                    {
                        piso.IdPiso = id;
                    }
                    var result = await _pisoRepository.UpdateAsync(id, piso);
            
                    if (result.IsSuccess)
                    {
                        TempData["Success"] = result.Message ?? "Piso actualizado correctamente.";
                        return RedirectToAction(nameof(Index));
                    }
            
                    TempData["Error"] = result.Message ?? "Error al actualizar el piso.";
                }
        
                return View(piso);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return View(piso);
            }
        }

        // GET: PisoController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var piso = await _pisoRepository.GetByIdAsync(id);
                return View(piso);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: PisoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                var result = await _pisoRepository.DeleteAsync(id);
        
                if (result.IsSuccess)
                {
                    TempData["Success"] = result.Message ?? "Piso eliminado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
        
                if (result.Message?.Contains("habitaciones asociadas") == true || 
                    result.Message?.Contains("relacionado con habitaciones") == true ||
                    result.Message?.Contains("no se puede eliminar") == true)
                {
                    TempData["Error"] = "No se puede eliminar el piso porque tiene habitaciones asociadas. Debe eliminar o reubicar las habitaciones primero.";
                }
                else
                {
                    TempData["Error"] = result.Message ?? "Error al eliminar el piso.";
                }
        
                if (Request.Path.Value?.Contains("/Delete/") == true)
                {
                    return RedirectToAction(nameof(Delete), new { id });
                }
        
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: PisoController/GetByDescripcion
        public async Task<IActionResult> GetByDescripcion(string descripcion)
        {
            try
            {
                if (string.IsNullOrEmpty(descripcion))
                {
                    TempData["Error"] = "La descripción no puede ser nula o vacía.";
                    return RedirectToAction(nameof(Index));
                }

                var piso = await _pisoRepository.GetByDescripcionAsync(descripcion);
                
                if (piso == null)
                {
                    TempData["Error"] = $"No se encontró un piso con la descripción: {descripcion}";
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