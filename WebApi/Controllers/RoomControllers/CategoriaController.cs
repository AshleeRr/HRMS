using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApi.Interfaces;
using WebApi.Interfaces.RoomInterface;
using WebApi.Models.RoomModels;
using WebApi.Models.Servicios;
namespace WebApi.Controllers.RoomControllers
{
    public class CategoriaController : Controller
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IApiClient _apiClient;

        public CategoriaController(ICategoriaRepository categoriaRepository, IApiClient apiClient)
        {
            _categoriaRepository = categoriaRepository;
            _apiClient = apiClient;
        }

        // GET: CategoriaController
        public async Task<IActionResult> Index()
        {
            List<CategoriaModel> categorias = new List<CategoriaModel>();
            try
            {
                categorias = (await _categoriaRepository.GetAllAsync()).ToList();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
            }
            
            return View(categorias);
        }

        // GET: CategoriaController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var categoria = await _categoriaRepository.GetByIdAsync(id);
                await CargarServicios();
                return View(categoria);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: CategoriaController/Create
        public async Task<IActionResult> Create()
        {
            await CargarServicios();
            return View(new CategoriaModel());
        }

        // POST: CategoriaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoriaModel categoria)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _categoriaRepository.CreateAsync(categoria);
                    
                    if (result.IsSuccess)
                    {
                        TempData["Success"] = result.Message ?? "Categoría creada correctamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    
                    TempData["Error"] = result.Message ?? "Error al crear la categoría.";
                }
                
                await CargarServicios();
                return View(categoria);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                await CargarServicios();
                return View(categoria);
            }
        }

        // GET: CategoriaController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var categoria = await _categoriaRepository.GetByIdAsync(id);
                
                if (categoria != null && categoria.IdCategoria != id)
                {
                    categoria.IdCategoria = id;
                }
                
                await CargarServicios();
                return View(categoria);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: CategoriaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoriaModel categoria)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (categoria.IdCategoria != id)
                    {
                        categoria.IdCategoria = id;
                    }
                    
                    categoria.ChangeTime = DateTime.Now;
                    
                    var result = await _categoriaRepository.UpdateAsync(id, categoria);
                    
                    if (result.IsSuccess)
                    {
                        TempData["Success"] = result.Message ?? "Categoría actualizada correctamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    
                    TempData["Error"] = result.Message ?? "Error al actualizar la categoría.";
                }
                
                await CargarServicios();
                return View(categoria);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                await CargarServicios();
                return View(categoria);
            }
        }

        // GET: CategoriaController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var categoria = await _categoriaRepository.GetByIdAsync(id);
                await CargarServicios();
                return View(categoria);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: CategoriaController/Delete/5
        // POST: CategoriaController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                var result = await _categoriaRepository.DeleteAsync(id);
        
                if (result.IsSuccess)
                {
                    TempData["Success"] = result.Message ?? "Categoría eliminada correctamente.";
                    return RedirectToAction(nameof(Index));
                }
        
                if (result.Message?.Contains("habitaciones asociadas") == true || 
                    result.Message?.Contains("no se puede eliminar") == true)
                {
                    TempData["Error"] = result.Message;
                }
                else
                {
                    TempData["Error"] = result.Message ?? "Error al eliminar la categoría.";
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

        // GET: CategoriaController/GetByDescripcion
        public async Task<IActionResult> GetByDescripcion(string descripcion)
        {
            List<CategoriaModel> categorias = new List<CategoriaModel>();
            try
            {
                var categoria = await _categoriaRepository.GetByDescripcionAsync(descripcion);
                
                if (categoria != null)
                {
                    categorias.Add(categoria);
                    ViewBag.TituloLista = $"Categoría con descripción: {descripcion}";
                    return View("Index", categorias);
                }
                
                TempData["Error"] = "No se encontró la categoría.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: CategoriaController/GetByCapacidad
        public async Task<IActionResult> GetByCapacidad(int capacidad)
        {
            try
            {
                var categorias = await _categoriaRepository.GetByCapacidadAsync(capacidad);
                
                if (categorias != null && categorias.Any())
                {
                    ViewBag.TituloLista = $"Categorías por capacidad: {capacidad}";
                    return View("Index", categorias);
                }
                
                TempData["Error"] = "No se encontraron categorías con esa capacidad.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // Método auxiliar para cargar servicios
        private async Task CargarServicios()
        {
            try
            {
                //No disponemos de un servicio para cargar los servicios, por lo que se hace una llamada a la API
                var servicios = await _apiClient.GetAsync<IEnumerable<ServicioModel>>("Servicio/GetAllServicios");
                
                if (servicios != null && servicios.Any())
                {
                    var selectItems = servicios.Select(s => new SelectListItem
                    {
                        Value = s.IdServicio.ToString(),
                        Text = s.Nombre
                    }).ToList();
                    
                    ViewBag.Servicios = selectItems;
                    return;
                }
                
                SetDefaultServices();
            }
            catch
            {
                SetDefaultServices();
            }
        }
        
        private void SetDefaultServices()
        {
            ViewBag.Servicios = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "-- Sin servicio --" },
                new SelectListItem { Value = "1", Text = "Desayuno" },
                new SelectListItem { Value = "2", Text = "Todo Incluido" },
                new SelectListItem { Value = "3", Text = "Básico" }
            };
        }
    }
}