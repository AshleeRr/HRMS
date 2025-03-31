using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
using WebApi.Models;
using WebApi.Models.RoomModels;
using WebApi.Models.Servicios;

namespace WebApi.Controllers.RoomControllers
{
    public class CategoriaController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;

        public CategoriaController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _apiBaseUrl = "https://localhost:7175/api";
        }

        // GET: CategoriaController
        public async Task<ActionResult> Index()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.GetAsync($"{_apiBaseUrl}/Categoria/GetAllCategorias");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var categorias = JsonConvert.DeserializeObject<List<CategoriaModel>>(content);
                    
                    TempData["Success"] = TempData["Success"]; // Preservar mensajes
                    return View(categorias);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var errorObj = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                        TempData["Error"] = errorObj?.detail ?? $"Error al obtener categorías: {response.ReasonPhrase}";
                    }
                    catch
                    {
                        TempData["Error"] = $"Error al obtener categorías: {response.ReasonPhrase}";
                    }
                    return View(new List<CategoriaModel>());
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return View(new List<CategoriaModel>());
            }
        }

        // GET: CategoriaController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.GetAsync($"{_apiBaseUrl}/Categoria/GetCategoriaById{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var categoria = JsonConvert.DeserializeObject<CategoriaModel>(content);
                    
                    await CargarServicios();
                    return View(categoria);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var errorObj = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                        TempData["Error"] = errorObj?.detail ?? $"Error al obtener los detalles de la categoría: {response.ReasonPhrase}";
                    }
                    catch
                    {
                        TempData["Error"] = $"Error al obtener los detalles de la categoría: {response.ReasonPhrase}";
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

        // GET: CategoriaController/Create
        public async Task<ActionResult> Create()
        {
            await CargarServicios();
            return View(new CategoriaModel
            {
            });
        }

        // POST: CategoriaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CategoriaModel categoria)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var client = _httpClientFactory.CreateClient();
                    var json = JsonConvert.SerializeObject(categoria);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync($"{_apiBaseUrl}/Categoria/CreateCategoria", content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Categoría creada correctamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var errorObj = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                            TempData["Error"] = errorObj?.detail ?? $"Error al crear la categoría: {errorContent}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al crear la categoría: {errorContent}";
                        }
                        
                        await CargarServicios();
                        return View(categoria);
                    }
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
        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.GetAsync($"{_apiBaseUrl}/Categoria/GetCategoriaById{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var categoria = JsonConvert.DeserializeObject<CategoriaModel>(content);
                    
                    if (categoria != null && categoria.IdCategoria != id)
                    {
                        categoria.IdCategoria = id;
                    }
                    
                    await CargarServicios();
                    return View(categoria);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var errorObj = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                        TempData["Error"] = errorObj?.detail ?? $"Error al obtener la categoría: {response.ReasonPhrase}";
                    }
                    catch
                    {
                        TempData["Error"] = $"Error al obtener la categoría: {response.ReasonPhrase}";
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

        // POST: CategoriaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, CategoriaModel categoria)
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
                    
                    var client = _httpClientFactory.CreateClient();
                    var json = JsonConvert.SerializeObject(categoria);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync($"{_apiBaseUrl}/Categoria/UpdateCategoriaById{id}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Categoría actualizada correctamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var errorObj = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                            TempData["Error"] = errorObj?.detail ?? $"Error al actualizar la categoría: {errorContent}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al actualizar la categoría: {errorContent}";
                        }
                        
                        await CargarServicios();
                        return View(categoria);
                    }
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
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.GetAsync($"{_apiBaseUrl}/Categoria/GetCategoriaById{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var categoria = JsonConvert.DeserializeObject<CategoriaModel>(content);
                    
                    await CargarServicios();
                    return View(categoria);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var errorObj = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                        TempData["Error"] = errorObj?.detail ?? $"Error al obtener la categoría: {response.ReasonPhrase}";
                    }
                    catch
                    {
                        TempData["Error"] = $"Error al obtener la categoría: {response.ReasonPhrase}";
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

        // POST: CategoriaController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.DeleteAsync($"{_apiBaseUrl}/Categoria/DeleteCategoriaById{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Categoría eliminada correctamente.";
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
                            TempData["Error"] = "No se puede eliminar la categoría porque tiene habitaciones asociadas.";
                        }
                        else
                        {
                            TempData["Error"] = errorObj?.detail ?? "Error al eliminar la categoría.";
                        }
                    }
                    catch
                    {
                        TempData["Error"] = "No se puede eliminar la categoría en este momento.";
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

        // GET: CategoriaController/GetByDescripcion
        public async Task<ActionResult> GetByDescripcion(string descripcion)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.GetAsync($"{_apiBaseUrl}/Categoria/GetCategoriaByDescripcion/{descripcion}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var categoria = JsonConvert.DeserializeObject<CategoriaModel>(content);
                    
                    var categorias = new List<CategoriaModel> { categoria };
                    ViewBag.TituloLista = $"Categoría con descripción: {descripcion}";
                    return View("Index", categorias);
                }
                else
                {
                    TempData["Error"] = $"Error al buscar categoría: {response.ReasonPhrase}";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: CategoriaController/GetByCapacidad
        public async Task<ActionResult> GetByCapacidad(int capacidad)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.GetAsync($"{_apiBaseUrl}/Categoria/GetHabitacionByCapacidad/{capacidad}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var categorias = JsonConvert.DeserializeObject<List<CategoriaModel>>(content);
                    
                    ViewBag.TituloLista = $"Categorías por capacidad: {capacidad}";
                    return View("Index", categorias);
                }
                else
                {
                    TempData["Error"] = $"Error al buscar categorías: {response.ReasonPhrase}";
                    return RedirectToAction(nameof(Index));
                }
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
                var client = _httpClientFactory.CreateClient();
              
                HttpResponseMessage responseServicios = await client.GetAsync($"{_apiBaseUrl}/Servicio/GetAllServicios");
                
                if (responseServicios.IsSuccessStatusCode)
                {
                    var contentServicios = await responseServicios.Content.ReadAsStringAsync();
                    var servicios = JsonConvert.DeserializeObject<List<ServicioModel>>(contentServicios);
                    
                    ViewBag.Servicios = servicios.Select(s => new SelectListItem
                    {
                        Value = s.IdServicio.ToString(),
                        Text = s.Nombre
                    }).ToList();
                }
                else
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
            catch
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
}