using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApi.Interfaces.RoomInterface;
using WebApi.Models.RoomModels;

namespace WebApi.Controllers.RoomControllers
{
    public class TarifaController : Controller
    {
        private readonly ITarifaRepository _tarifaRepository;
        private readonly ICategoriaRepository _categoriaRepository;

        public TarifaController(ITarifaRepository tarifaRepository, ICategoriaRepository categoriaRepository)
        {
            _tarifaRepository = tarifaRepository;
            _categoriaRepository = categoriaRepository;
        }
        
        // GET: TarifaController
        public async Task<IActionResult> Index()
        {
            try
            {
                var tarifas = await _tarifaRepository.GetAllAsync();
                ViewBag.TituloLista = "Lista de Tarifas";
                return View(tarifas);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar tarifas: {ex.Message}";
                return View(new List<TarifaModel>());
            }
        }

        // GET: TarifaController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var tarifa = await _tarifaRepository.GetByIdAsync(id);
                
                if (tarifa == null || tarifa.IdTarifa <= 0)
                {
                    TempData["Error"] = "Tarifa no encontrada.";
                    return RedirectToAction(nameof(Index));
                }
                
                return View(tarifa);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar detalles: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: TarifaController/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                await CargarCategorias();
                return View(new TarifaModel());
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al preparar formulario: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: TarifaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TarifaModel tarifa)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await CargarCategorias();
                    return View(tarifa);
                }
                
                var result = await _tarifaRepository.CreateAsync(tarifa);
                
                if (result.IsSuccess)
                {
                    TempData["Success"] = result.Message ?? "Tarifa creada correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                
                TempData["Error"] = result.Message ?? "Error al crear la tarifa.";
                await CargarCategorias();
                return View(tarifa);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                await CargarCategorias();
                return View(tarifa);
            }
        }

        // GET: TarifaController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var tarifa = await _tarifaRepository.GetByIdAsync(id);
                
                if (tarifa == null || tarifa.IdTarifa <= 0)
                {
                    TempData["Error"] = "Tarifa no encontrada.";
                    return RedirectToAction(nameof(Index));
                }
                
                await CargarCategorias();
                return View(tarifa);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar formulario: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: TarifaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TarifaModel tarifa)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await CargarCategorias();
                    return View(tarifa);
                }
                
                // Asegurar que el ID sea correcto
                if (tarifa.IdTarifa != id)
                {
                    tarifa.IdTarifa = id;
                }
                
                var result = await _tarifaRepository.UpdateAsync(id, tarifa);
                
                if (result.IsSuccess)
                {
                    TempData["Success"] = result.Message ?? "Tarifa actualizada correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                
                TempData["Error"] = result.Message ?? "Error al actualizar la tarifa.";
                await CargarCategorias();
                return View(tarifa);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                await CargarCategorias();
                return View(tarifa);
            }
        }

        // GET: TarifaController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var tarifa = await _tarifaRepository.GetByIdAsync(id);
                
                if (tarifa == null || tarifa.IdTarifa <= 0)
                {
                    TempData["Error"] = "Tarifa no encontrada.";
                    return RedirectToAction(nameof(Index));
                }
                
                return View(tarifa);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar tarifa: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: TarifaController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _tarifaRepository.DeleteAsync(id);
                
                if (result.IsSuccess)
                {
                    TempData["Success"] = result.Message ?? "Tarifa eliminada correctamente.";
                }
                else
                {
                    TempData["Error"] = result.Message ?? "Error al eliminar la tarifa.";
                }
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: TarifaController/GetByFecha
        public async Task<IActionResult> GetByFecha(DateTime fecha)
        {
            try
            {
                var tarifas = await _tarifaRepository.GetTarifaByFecha(fecha);
                ViewBag.TituloLista = $"Tarifas para la fecha: {fecha.ToShortDateString()}";
                return View("Index", tarifas);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al buscar tarifas por fecha: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: TarifaController/GetByPrecio
        public async Task<IActionResult> GetByPrecio(decimal precio)
        {
            try
            {
                var tarifas = await _tarifaRepository.GetTarifaByPrecio(precio);
                ViewBag.TituloLista = $"Tarifas para el precio: {precio:C}";
                return View("Index", tarifas);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al buscar tarifas por precio: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task CargarCategorias()
        {
            try
            {
                var categorias = await _categoriaRepository.GetAllAsync();
                
                bool usarNombre = categorias?.FirstOrDefault()?.GetType().GetProperty("Nombre") != null;
                
                if (categorias != null && categorias.Any())
                {
                    if (usarNombre)
                    {
                        ViewBag.Categorias = new SelectList(categorias, "IdCategoria", "Nombre");
                    }
                    else
                    {
                        ViewBag.Categorias = new SelectList(categorias, "IdCategoria", "Descripcion");
                    }
                }
                else
                {
                    ViewBag.Categorias = new SelectList(new List<CategoriaModel>(), "IdCategoria", "Descripcion");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Categorias = new SelectList(new List<CategoriaModel>(), "IdCategoria", "Descripcion");
            }
        }
    }
}