using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
using WebApi.Models;
using WebApi.Models.RoomModels;

namespace WebApi.Controllers.RoomControllers
{
    public class PisoController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;

        public PisoController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _apiBaseUrl = "https://localhost:7175/api";
        }

        // GET: PisoController
        public async Task<IActionResult> Index()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.GetAsync($"{_apiBaseUrl}/Piso/GetAllPisos");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var pisos = JsonConvert.DeserializeObject<List<PisoModel>>(content);
                    
                    TempData["Success"] = TempData["Success"]; // Preservar mensajes
                    return View(pisos);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var errorObj = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                        TempData["Error"] = errorObj?.detail ?? $"Error al obtener pisos: {response.ReasonPhrase}";
                    }
                    catch
                    {
                        TempData["Error"] = $"Error al obtener pisos: {response.ReasonPhrase}";
                    }
                    return View(new List<PisoModel>());
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return View(new List<PisoModel>());
            }
        }

        // GET: PisoController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.GetAsync($"{_apiBaseUrl}/Piso/GetPisoById{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var piso = JsonConvert.DeserializeObject<PisoModel>(content);
                    
                    return View(piso);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var errorObj = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                        TempData["Error"] = errorObj?.detail ?? $"Error al obtener los detalles del piso: {response.ReasonPhrase}";
                    }
                    catch
                    {
                        TempData["Error"] = $"Error al obtener los detalles del piso: {response.ReasonPhrase}";
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

        // GET: PisoController/Create
        public ActionResult Create()
        {
            return View(new PisoModel
            {

            });
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
                    var client = _httpClientFactory.CreateClient();
                    var json = JsonConvert.SerializeObject(piso);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync($"{_apiBaseUrl}/Piso/CreatePiso", content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Piso creado correctamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var errorObj = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                            TempData["Error"] = errorObj?.detail ?? $"Error al crear el piso: {errorContent}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al crear el piso: {errorContent}";
                        }
                        return View(piso);
                    }
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
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.GetAsync($"{_apiBaseUrl}/Piso/GetPisoById{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var piso = JsonConvert.DeserializeObject<PisoModel>(content);
                    
                    if (piso != null && piso.IdPiso != id)
                    {
                        piso.IdPiso = id;
                    }
                    
                    return View(piso);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var errorObj = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                        TempData["Error"] = errorObj?.detail ?? $"Error al obtener el piso: {response.ReasonPhrase}";
                    }
                    catch
                    {
                        TempData["Error"] = $"Error al obtener el piso: {response.ReasonPhrase}";
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
                    
                    piso.ChangeTime = DateTime.Now;
                    
                    var client = _httpClientFactory.CreateClient();
                    var json = JsonConvert.SerializeObject(piso);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PatchAsync($"{_apiBaseUrl}/Piso/UpdatePiso{id}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Piso actualizado correctamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var errorObj = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                            TempData["Error"] = errorObj?.detail ?? $"Error al actualizar el piso: {errorContent}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al actualizar el piso: {errorContent}";
                        }
                        return View(piso);
                    }
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
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.GetAsync($"{_apiBaseUrl}/Piso/GetPisoById{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var piso = JsonConvert.DeserializeObject<PisoModel>(content);
                    
                    return View(piso);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var errorObj = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                        TempData["Error"] = errorObj?.detail ?? $"Error al obtener el piso: {response.ReasonPhrase}";
                    }
                    catch
                    {
                        TempData["Error"] = $"Error al obtener el piso: {response.ReasonPhrase}";
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

        // POST: PisoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.DeleteAsync($"{_apiBaseUrl}/Piso/DeletePiso{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Piso eliminado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var errorObj = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                        
                        if (errorObj?.detail?.Contains("habitaciones asociadas") == true || 
                            errorObj?.detail?.Contains("relacionado con habitaciones") == true)
                        {
                            TempData["Error"] = "No se puede eliminar el piso porque tiene habitaciones asociadas.";
                        }
                        else
                        {
                            TempData["Error"] = errorObj?.detail ?? "Error al eliminar el piso.";
                        }
                    }
                    catch
                    {
                        TempData["Error"] = "No se puede eliminar el piso en este momento.";
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

        // GET: PisoController/GetByDescripcion
        public async Task<IActionResult> GetByDescripcion(string descripcion)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.GetAsync($"{_apiBaseUrl}/Piso/GetPisoByDescripcion{descripcion}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var piso = JsonConvert.DeserializeObject<PisoModel>(content);
                    
                    var pisos = new List<PisoModel> { piso };
                    ViewBag.TituloLista = $"Piso con descripción: {descripcion}";
                    return View("Index", pisos);
                }
                else
                {
                    TempData["Error"] = $"Error al buscar piso: {response.ReasonPhrase}";
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