using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using WebApi.Models;
using WebApi.Models.RoomModels;

namespace WebApi.Controllers.RoomControllers
{
    public class EstadoHabitacionController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;

        public EstadoHabitacionController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _apiBaseUrl = "https://localhost:7175/api";
        }

        // GET: EstadoHabitacionController
        public async Task<IActionResult> Index()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.GetAsync($"{_apiBaseUrl}/EstadoHabitacion/GetEstadoHabitaciones");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var estados = JsonConvert.DeserializeObject<List<EstadoHabitacionModel>>(content);
                    
                    TempData["Success"] = TempData["Success"]; 
                    return View(estados);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var errorObj = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                        TempData["Error"] = errorObj?.detail ?? $"Error al obtener estados: {response.ReasonPhrase}";
                    }
                    catch
                    {
                        TempData["Error"] = $"Error al obtener estados: {response.ReasonPhrase}";
                    }
                    return View(new List<EstadoHabitacionModel>());
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return View(new List<EstadoHabitacionModel>());
            }
        }

        // GET: EstadoHabitacionController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.GetAsync($"{_apiBaseUrl}/EstadoHabitacion/GetEstadoBy(id){id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var estado = JsonConvert.DeserializeObject<EstadoHabitacionModel>(content);
                    
                    return View(estado);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var errorObj = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                        TempData["Error"] = errorObj?.detail ?? $"Error al obtener los detalles del estado: {response.ReasonPhrase}";
                    }
                    catch
                    {
                        TempData["Error"] = $"Error al obtener los detalles del estado: {response.ReasonPhrase}";
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

        // GET: EstadoHabitacionController/Create
        public ActionResult Create()
        {
            return View(new EstadoHabitacionModel
            {
                
            });
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
                    var client = _httpClientFactory.CreateClient();
                    var json = JsonConvert.SerializeObject(estado);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync($"{_apiBaseUrl}/EstadoHabitacion/CreateEstadoHabitacion", content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Estado de habitación creado correctamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var errorObj = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                            TempData["Error"] = errorObj?.detail ?? $"Error al crear el estado: {errorContent}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al crear el estado: {errorContent}";
                        }
                        return View(estado);
                    }
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
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.GetAsync($"{_apiBaseUrl}/EstadoHabitacion/GetEstadoBy(id){id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var estado = JsonConvert.DeserializeObject<EstadoHabitacionModel>(content);
                    
                    if (estado != null && estado.IdEstadoHabitacion != id)
                    {
                        estado.IdEstadoHabitacion = id;
                    }
                    
                    return View(estado);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var errorObj = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                        TempData["Error"] = errorObj?.detail ?? $"Error al obtener el estado: {response.ReasonPhrase}";
                    }
                    catch
                    {
                        TempData["Error"] = $"Error al obtener el estado: {response.ReasonPhrase}";
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
                    
                    estado.ChangeTime = DateTime.Now;
                    
                    var client = _httpClientFactory.CreateClient();
                    var json = JsonConvert.SerializeObject(estado);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"{_apiBaseUrl}/EstadoHabitacion/UpdateEstadoHabitacionById{id}");
                    request.Content = content;
                    
                    HttpResponseMessage response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Estado de habitación actualizado correctamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var errorObj = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                            TempData["Error"] = errorObj?.detail ?? $"Error al actualizar el estado: {errorContent}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al actualizar el estado: {errorContent}";
                        }
                        return View(estado);
                    }
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
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.GetAsync($"{_apiBaseUrl}/EstadoHabitacion/GetEstadoBy(id){id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var estado = JsonConvert.DeserializeObject<EstadoHabitacionModel>(content);
                    
                    return View(estado);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var errorObj = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                        TempData["Error"] = errorObj?.detail ?? $"Error al obtener el estado: {response.ReasonPhrase}";
                    }
                    catch
                    {
                        TempData["Error"] = $"Error al obtener el estado: {response.ReasonPhrase}";
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

        // POST: EstadoHabitacionController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.DeleteAsync($"{_apiBaseUrl}/EstadoHabitacion/DeleteEstadoHabitacionById{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Estado de habitación eliminado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var errorObj = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                        
                        if (errorObj?.detail?.Contains("habitaciones asociadas") == true)
                        {
                            TempData["Error"] = "No se puede eliminar el estado porque tiene habitaciones asociadas.";
                        }
                        else
                        {
                            TempData["Error"] = errorObj?.detail ?? "Error al eliminar el estado.";
                        }
                    }
                    catch
                    {
                        TempData["Error"] = "No se puede eliminar el estado en este momento.";
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

        // GET: EstadoHabitacionController/GetByDescripcion
        public async Task<ActionResult> GetByDescripcion(string descripcion)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.GetAsync($"{_apiBaseUrl}/EstadoHabitacion/GetEstadoBy(descripcion){descripcion}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var estado = JsonConvert.DeserializeObject<EstadoHabitacionModel>(content);
                    
                    var estados = new List<EstadoHabitacionModel> { estado };
                    ViewBag.TituloLista = $"Estado con descripción: {descripcion}";
                    return View("Index", estados);
                }
                else
                {
                    TempData["Error"] = $"Error al buscar estado: {response.ReasonPhrase}";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}