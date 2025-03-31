using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
using WebApi.Models;
using WebApi.Models.RoomModels;

namespace WebApi.Controllers.RoomControllers
{
    public class HabitacionController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;

        public HabitacionController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _apiBaseUrl = "https://localhost:7175/api";
        }

        // GET: HabitacionController
        public async Task<IActionResult> Index()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.GetAsync($"{_apiBaseUrl}/Habitacion/GetAllHabitaciones");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var habitaciones = JsonConvert.DeserializeObject<List<HabitacionModel>>(content);
                    
                    TempData["Success"] = TempData["Success"]; 
                    return View(habitaciones);
                }
                else
                {
                    TempData["Error"] = $"Error al obtener habitaciones: {response.ReasonPhrase}";
                    return View(new List<HabitacionModel>());
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return View(new List<HabitacionModel>());
            }
        }

        // GET: HabitacionController/InfoHabitaciones
        public async Task<IActionResult> InfoHabitaciones()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.GetAsync($"{_apiBaseUrl}/Habitacion/GetInfoHabitaciones");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var habitacionesInfo = JsonConvert.DeserializeObject<List<HabitacionInfoModel>>(content);
                    
                    return View(habitacionesInfo);
                }
                else
                {
                    TempData["Error"] = $"Error al obtener información detallada: {response.ReasonPhrase}";
                    return View(new List<HabitacionInfoModel>());
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return View(new List<HabitacionInfoModel>());
            }
        }

        // GET: HabitacionController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.GetAsync($"{_apiBaseUrl}/Habitacion/GetByHabitacionById{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var habitacion = JsonConvert.DeserializeObject<HabitacionModel>(content);
                    
                    return View(habitacion);
                }
                else
                {
                    TempData["Error"] = $"Error al obtener detalles de la habitación: {response.ReasonPhrase}";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: HabitacionController/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                await CargarListasDesplegables();
                
                return View(new HabitacionModel
                {
                });
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
                    var client = _httpClientFactory.CreateClient();
                    var json = JsonConvert.SerializeObject(habitacion);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync($"{_apiBaseUrl}/Habitacion/CreateHabitacion", content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Habitación creada correctamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        TempData["Error"] = $"Error al crear habitación: {errorContent}";
                        
                        await CargarListasDesplegables();
                        return View(habitacion);
                    }
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
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.GetAsync($"{_apiBaseUrl}/Habitacion/GetByHabitacionById{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var habitacion = JsonConvert.DeserializeObject<HabitacionModel>(content);
                    
                   
                    if (habitacion != null && habitacion.idHabitacion != id)
                    {
                        habitacion.idHabitacion = id;
                    }
                    
                    await CargarListasDesplegables();
                    
                    return View(habitacion);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    
                    try
                    {
                        var errorObj = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                        TempData["Error"] = errorObj?.detail ?? $"Error al obtener la habitación: {response.ReasonPhrase}";
                    }
                    catch
                    {
                        TempData["Error"] = $"Error al obtener la habitación: {response.ReasonPhrase}";
                    }
                    
                    return RedirectToAction(nameof(Index));
                }
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
                 
                    if (habitacion.idHabitacion != id)
                    {
                        habitacion.idHabitacion = id;
                    }
                    
                    habitacion.ChangeTime = DateTime.Now;
                    
                    var client = _httpClientFactory.CreateClient();
                    var json = JsonConvert.SerializeObject(habitacion);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync($"{_apiBaseUrl}/Habitacion/(UpdateHabitacionBy){id}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Habitación actualizada correctamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        
                        try
                        {
                            var errorObj = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                            TempData["Error"] = errorObj?.detail ?? "Error al actualizar la habitación.";
                        }
                        catch
                        {
                            TempData["Error"] = "No se puede actualizar la habitación en este momento.";
                        }
                        
                        await CargarListasDesplegables();
                        return View(habitacion);
                    }
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
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.GetAsync($"{_apiBaseUrl}/Habitacion/GetByHabitacionById{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var habitacion = JsonConvert.DeserializeObject<HabitacionModel>(content);
                    
                    return View(habitacion);
                }
                else
                {
                    TempData["Error"] = $"Error al obtener la habitación: {response.ReasonPhrase}";
                    return RedirectToAction(nameof(Index));
                }
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
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.DeleteAsync($"{_apiBaseUrl}/Habitacion/DeleteHabitacionBy{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Habitación eliminada correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    
                    try
                    {
                        var errorObj = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                        
                        if (errorObj?.detail?.Contains("reservas activas") == true)
                        {
                            TempData["Error"] = "No se puede eliminar la habitación porque tiene reservas activas.";
                        }
                        else
                        {
                            TempData["Error"] = errorObj?.detail ?? "Error al eliminar la habitación.";
                        }
                    }
                    catch
                    {
                        TempData["Error"] = "No se puede eliminar la habitación en este momento.";
                    }
                    
                    if (Request.Path.Value?.Contains("/Delete/") == true)
                    {
                        return RedirectToAction(nameof(Delete), new { id });
                    }
                    
                    return RedirectToAction(nameof(Index));
                }
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
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.GetAsync($"{_apiBaseUrl}/Habitacion/GetHabitacionByPiso/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var habitaciones = JsonConvert.DeserializeObject<List<HabitacionModel>>(content);
                    
                    ViewBag.TituloLista = $"Habitaciones en el Piso {id}";
                    return View("Filtered", habitaciones);
                }
                else
                {
                    TempData["Error"] = $"Error al filtrar habitaciones: {response.ReasonPhrase}";
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
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.GetAsync($"{_apiBaseUrl}/Habitacion/GetHabitacionByCategoria/{categoria}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var habitaciones = JsonConvert.DeserializeObject<List<HabitacionModel>>(content);
                    
                    ViewBag.TituloLista = $"Habitaciones de Categoría: {categoria}";
                    return View("Filtered", habitaciones);
                }
                else
                {
                    TempData["Error"] = $"Error al filtrar habitaciones: {response.ReasonPhrase}";
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
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.GetAsync($"{_apiBaseUrl}/Habitacion/GetHabitacionBy/{numero}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var habitacion = JsonConvert.DeserializeObject<HabitacionModel>(content);
                    
                    var habitaciones = new List<HabitacionModel> { habitacion };
                    ViewBag.TituloLista = $"Habitación Número: {numero}";
                    return View("Filtered", habitaciones);
                }
                else
                {
                    TempData["Error"] = $"Error al buscar habitación: {response.ReasonPhrase}";
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
            var client = _httpClientFactory.CreateClient();
            
            try
            {
                HttpResponseMessage responseEstados = await client.GetAsync($"{_apiBaseUrl}/EstadoHabitacion/GetEstadoHabitaciones");
                if (responseEstados.IsSuccessStatusCode)
                {
                    var contentEstados = await responseEstados.Content.ReadAsStringAsync();
                    var estados = JsonConvert.DeserializeObject<List<EstadoHabitacionModel>>(contentEstados);
                    
                    ViewBag.EstadosHabitacion = estados.Select(e => new SelectListItem
                    {
                        Value = e.IdEstadoHabitacion.ToString(),
                        Text = e.Descripcion
                    }).ToList();
                }
            }
            catch (Exception)
            {
                ViewBag.EstadosHabitacion = null;
            }

            try
            {
                HttpResponseMessage responsePisos = await client.GetAsync($"{_apiBaseUrl}/Piso/GetAllPisos");
                if (responsePisos.IsSuccessStatusCode)
                {
                    var contentPisos = await responsePisos.Content.ReadAsStringAsync();
                    var pisos = JsonConvert.DeserializeObject<List<PisoModel>>(contentPisos);
                    
                    ViewBag.Pisos = pisos.Select(p => new SelectListItem
                    {
                        Value = p.IdPiso.ToString(),
                        Text = p.Descripcion
                    }).ToList();
                }
            }
            catch (Exception)
            {
                ViewBag.Pisos = null;
            }

            try
            {
                HttpResponseMessage responseCategorias = await client.GetAsync($"{_apiBaseUrl}/Categoria/GetAllCategorias");
                if (responseCategorias.IsSuccessStatusCode)
                {
                    var contentCategorias = await responseCategorias.Content.ReadAsStringAsync();
                    var categorias = JsonConvert.DeserializeObject<List<CategoriaModel>>(contentCategorias);
                    
                    ViewBag.Categorias = categorias.Select(c => new SelectListItem
                    {
                        Value = c.IdCategoria.ToString(),
                        Text = c.Descripcion
                    }).ToList();
                }
            }
            catch (Exception)
            {
                ViewBag.Categorias = null;
            }
        }
    }
}