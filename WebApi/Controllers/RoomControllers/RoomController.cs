using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApi.Interfaces;
using WebApi.Interfaces.RoomInterface;
using WebApi.Models.RoomModels;

namespace WebApi.Controllers.RoomControllers
{
    public class HabitacionController : Controller
    {
        private readonly IHabitacionRepository _habitacionRepository;
        private readonly IEstadoHabitacionRepository _estadoHabitacionRepository;
        private readonly IPisoRepository _pisoRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        
        public HabitacionController(IHabitacionRepository habitacionRepository, IEstadoHabitacionRepository estadoHabitacionRepository, 
            IPisoRepository pisoRepository, ICategoriaRepository categoriaRepository)
        {
            _habitacionRepository = habitacionRepository;
            _estadoHabitacionRepository = estadoHabitacionRepository;
            _pisoRepository = pisoRepository;
            _categoriaRepository = categoriaRepository;
        }
        
        // GET: HabitacionController
        public async Task<IActionResult> Index()
        {
            try
            {
                var habitaciones = await _habitacionRepository.GetAllAsync();
                return View(habitaciones);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar habitaciones: {ex.Message}";
                return View(new List<HabitacionModel>());
            }
        }

        // GET: HabitacionController/InfoHabitaciones
        public async Task<IActionResult> InfoHabitaciones()
        {
            try
            {
                var habitacionesInfo = await _habitacionRepository.GetInfoHabitaciones();
                return View(habitacionesInfo);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar información de habitaciones: {ex.Message}";
                return View(new List<HabitacionInfoModel>());
            }
        }

        // GET: HabitacionController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var habitacion = await _habitacionRepository.GetByIdAsync(id);
                
                if (habitacion == null)
                {
                    TempData["Error"] = "Habitación no encontrada.";
                    return RedirectToAction(nameof(Index));
                }
                
                return View(habitacion);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar detalles: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: HabitacionController/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                await CargarListasDesplegables();
                return View(new HabitacionModel());
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al preparar el formulario: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: HabitacionController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HabitacionModel habitacion)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    habitacion.ChangeTime = DateTime.Now;
                    
                    var result = await _habitacionRepository.CreateAsync(habitacion);
                    
                    if (result.IsSuccess)
                    {
                        TempData["Success"] = result.Message ?? "Habitación creada correctamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    
                    TempData["Error"] = result.Message ?? "Error al crear la habitación.";
                }
                
                await CargarListasDesplegables();
                return View(habitacion);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                await CargarListasDesplegables();
                return View(habitacion);
            }
        }

        // GET: HabitacionController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var habitacion = await _habitacionRepository.GetByIdAsync(id);
                
                if (habitacion == null)
                {
                    TempData["Error"] = "Habitación no encontrada.";
                    return RedirectToAction(nameof(Index));
                }
                
                await CargarListasDesplegables();
                return View(habitacion);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: HabitacionController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HabitacionModel habitacion)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    habitacion.ChangeTime = DateTime.Now;
                    
                    var result = await _habitacionRepository.UpdateAsync(id, habitacion);
                    
                    if (result.IsSuccess)
                    {
                        TempData["Success"] = result.Message ?? "Habitación actualizada correctamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    
                    TempData["Error"] = result.Message ?? "Error al actualizar la habitación.";
                }
                
                await CargarListasDesplegables();
                return View(habitacion);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                await CargarListasDesplegables();
                return View(habitacion);
            }
        }

        // GET: HabitacionController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var habitacion = await _habitacionRepository.GetByIdAsync(id);
                
                if (habitacion == null)
                {
                    TempData["Error"] = "Habitación no encontrada.";
                    return RedirectToAction(nameof(Index));
                }
                
                return View(habitacion);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: HabitacionController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                var result = await _habitacionRepository.DeleteAsync(id);
                if (result.IsSuccess)
                {
                    TempData["Success"] = result.Message ?? "Habitación eliminada correctamente.";
                    return RedirectToAction(nameof(Index));
                }

                if (result.Message?.Contains("Tiene reservas") == true)
                {
                    TempData["Error"] = "No se puede eliminar la habitación porque tiene reservas activas.";
                }
                else
                {
                    TempData["Error"] = result.Message ?? "Error al eliminar la habitación.";
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

        // GET: HabitacionController/FilterByPiso/1
        public async Task<IActionResult> FilterByPiso(int id)
        {
            try
            {
                var piso = await _pisoRepository.GetByIdAsync(id);
                string pisoDescripcion = piso?.Descripcion ?? $"Piso {id}";
                
                var result = await _habitacionRepository.GetByPisoAsync(id);
                if (result != null && result.Any())
                {
                    var habitaciones = result.ToList();
                    ViewBag.TituloLista = $"Habitaciones del {pisoDescripcion}";
                    return View("HabitacionFiltradas", habitaciones);
                }
                else
                {
                    TempData["Error"] = $"No se encontraron habitaciones en {pisoDescripcion}.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: HabitacionController/FilterByCategoria/1
        public async Task<IActionResult> FilterByCategoria(string categoria)
        {
            try
            {
                var result = await _habitacionRepository.GetByCategoriaAsync(categoria);
                if (result != null && result.Any())
                {
                    var habitaciones = result.ToList();
                    ViewBag.TituloLista = $"Habitaciones de la Categoría: {categoria}";
                    return View("HabitacionFiltradas", habitaciones);
                }
                else
                {
                    TempData["Error"] = "No se encontraron habitaciones en esta categoría.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: HabitacionController/FilterByNumero/101
        public async Task<IActionResult> FilterByNumero(string numero)
        {
            try
            {
                var result = await _habitacionRepository.GetByNumeroAsync(numero);
                if (result != null)
                {
                    ViewBag.TituloLista = $"Habitación: {numero}";
                    return View("HabitacionFiltradas", new List<HabitacionModel> { result });
                }
                else
                {
                    TempData["Error"] = "No se encontró la habitación.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task CargarListasDesplegables()
        {
            await CargarEstados();
            await CargarPisos();
            await CargarCategorias();
        }

        private async Task CargarEstados()
        {
            var estados = await _estadoHabitacionRepository.GetAllAsync();
            if (estados != null)
            {
                ViewBag.Estados = new SelectList(estados, "IdEstadoHabitacion", "Descripcion");
                ViewBag.EstadosHabitacion = new SelectList(estados, "IdEstadoHabitacion", "Descripcion");
            }
        }
        
        private async Task CargarPisos()
        {
            var pisos = await _pisoRepository.GetAllAsync();
            if (pisos != null)
            {
                ViewBag.Pisos = new SelectList(pisos, "IdPiso", "Descripcion");
            }
        }

        private async Task CargarCategorias()
        {
            var categorias = await _categoriaRepository.GetAllAsync();
            if (categorias != null)
            {
                ViewBag.Categorias = new SelectList(categorias, "IdCategoria", "Descripcion");
            }
        }
    }
}