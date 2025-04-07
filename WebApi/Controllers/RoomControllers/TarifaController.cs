using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
using WebApi.Models;
using WebApi.Models.RoomModels;

namespace WebApi.Controllers.RoomControllers
{
    public class TarifaController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;

        public TarifaController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _apiBaseUrl = "https://localhost:7175/api";
        }

        // GET: TarifaController
        public async Task<ActionResult> Index()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.GetAsync($"{_apiBaseUrl}/Tarifa/GetAllTarifas");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var tarifas = JsonConvert.DeserializeObject<List<TarifaModel>>(content);
                    
                    TempData["Success"] = TempData["Success"]; // Preservar mensajes
                    return View(tarifas);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var errorObj = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                        TempData["Error"] = errorObj?.detail ?? $"Error al obtener tarifas: {response.ReasonPhrase}";
                    }
                    catch
                    {
                        TempData["Error"] = $"Error al obtener tarifas: {response.ReasonPhrase}";
                    }
                    return View(new List<TarifaModel>());
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return View(new List<TarifaModel>());
            }
        }

        // GET: TarifaController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.GetAsync($"{_apiBaseUrl}/Tarifa/GetTarifaById{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var tarifa = JsonConvert.DeserializeObject<TarifaModel>(content);
                    
                    await CargarCategorias();
                    return View(tarifa);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var errorObj = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                        TempData["Error"] = errorObj?.detail ?? $"Error al obtener los detalles de la tarifa: {response.ReasonPhrase}";
                    }
                    catch
                    {
                        TempData["Error"] = $"Error al obtener los detalles de la tarifa: {response.ReasonPhrase}";
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

        // GET: TarifaController/Create
        public async Task<ActionResult> Create()
        {
            await CargarCategorias();
            return View(new TarifaModel
            {
                FechaInicio = DateTime.Today,
                FechaFin = DateTime.Today.AddDays(30),
                Descuento = 0,
                ChangeTime = DateTime.Now,
                UserID = 1 // Esto debería venir del usuario autenticado
            });
        }

        // POST: TarifaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TarifaModel tarifa)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (tarifa.FechaFin < tarifa.FechaInicio)
                    {
                        ModelState.AddModelError("FechaFin", "La fecha de fin no puede ser anterior a la fecha de inicio");
                        await CargarCategorias();
                        return View(tarifa);
                    }

                    var client = _httpClientFactory.CreateClient();
                    var json = JsonConvert.SerializeObject(tarifa);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync($"{_apiBaseUrl}/Tarifa/CreateTarifa", content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Tarifa creada correctamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var errorObj = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                            TempData["Error"] = errorObj?.detail ?? $"Error al crear la tarifa: {errorContent}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al crear la tarifa: {errorContent}";
                        }
                        
                        await CargarCategorias();
                        return View(tarifa);
                    }
                }
                
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
        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.GetAsync($"{_apiBaseUrl}/Tarifa/GetTarifaById{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var tarifa = JsonConvert.DeserializeObject<TarifaModel>(content);
                    
                    // Verificar que el ID coincida con el ID de la URL
                    if (tarifa != null && tarifa.IdTarifa != id)
                    {
                        tarifa.IdTarifa = id;
                    }
                    
                    await CargarCategorias();
                    return View(tarifa);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var errorObj = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                        TempData["Error"] = errorObj?.detail ?? $"Error al obtener la tarifa: {response.ReasonPhrase}";
                    }
                    catch
                    {
                        TempData["Error"] = $"Error al obtener la tarifa: {response.ReasonPhrase}";
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

        // POST: TarifaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, TarifaModel tarifa)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (tarifa.FechaFin < tarifa.FechaInicio)
                    {
                        ModelState.AddModelError("FechaFin", "La fecha de fin no puede ser anterior a la fecha de inicio");
                        await CargarCategorias();
                        return View(tarifa);
                    }

                    // Asegurar que el ID en la URL coincida con el ID en el modelo
                    if (tarifa.IdTarifa != id)
                    {
                        tarifa.IdTarifa = id;
                    }
                    
                    tarifa.ChangeTime = DateTime.Now;
                    
                    var client = _httpClientFactory.CreateClient();
                    var json = JsonConvert.SerializeObject(tarifa);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync($"{_apiBaseUrl}/Tarifa/UpdateTarifaBy{id}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Tarifa actualizada correctamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var errorObj = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                            TempData["Error"] = errorObj?.detail ?? $"Error al actualizar la tarifa: {errorContent}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al actualizar la tarifa: {errorContent}";
                        }
                        
                        await CargarCategorias();
                        return View(tarifa);
                    }
                }
                
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
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.GetAsync($"{_apiBaseUrl}/Tarifa/GetTarifaById{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var tarifa = JsonConvert.DeserializeObject<TarifaModel>(content);
                    
                    await CargarCategorias();
                    return View(tarifa);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var errorObj = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                        TempData["Error"] = errorObj?.detail ?? $"Error al obtener la tarifa: {response.ReasonPhrase}";
                    }
                    catch
                    {
                        TempData["Error"] = $"Error al obtener la tarifa: {response.ReasonPhrase}";
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

        // POST: TarifaController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.DeleteAsync($"{_apiBaseUrl}/Tarifa/DeleteTarifaBy{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Tarifa eliminada correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var errorObj = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                        
                        // Verificar si tiene reservas asociadas u otros problemas comunes
                        if (errorObj?.detail?.Contains("reservas asociadas") == true)
                        {
                            TempData["Error"] = "No se puede eliminar la tarifa porque tiene reservas asociadas.";
                        }
                        else
                        {
                            TempData["Error"] = errorObj?.detail ?? "Error al eliminar la tarifa.";
                        }
                    }
                    catch
                    {
                        TempData["Error"] = "No se puede eliminar la tarifa en este momento.";
                    }
                    
                    // Si estamos en la vista Delete, regresar a la vista con el objeto
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

        // GET: TarifaController/GetByFecha
        public async Task<ActionResult> GetByFecha(DateTime fecha)
        {
            try
            {
                var fechaFormateada = fecha.ToString("yyyy-MM-dd");
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.GetAsync($"{_apiBaseUrl}/Tarifa/GetTarifaByFecha?fecha={fechaFormateada}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var tarifas = JsonConvert.DeserializeObject<List<TarifaModel>>(content);
                    
                    ViewBag.TituloLista = $"Tarifas para la fecha: {fecha.ToString("dd/MM/yyyy")}";
                    return View("Index", tarifas);
                }
                else
                {
                    TempData["Error"] = $"Error al buscar tarifas: {response.ReasonPhrase}";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: TarifaController/GetByPrecio
        public async Task<ActionResult> GetByPrecio(decimal precio)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.GetAsync($"{_apiBaseUrl}/Tarifa/GetTarifaByPrecio/{precio}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var tarifas = JsonConvert.DeserializeObject<List<TarifaModel>>(content);
                    
                    ViewBag.TituloLista = $"Tarifas con precio: ${precio}";
                    return View("Index", tarifas);
                }
                else
                {
                    TempData["Error"] = $"Error al buscar tarifas: {response.ReasonPhrase}";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // Método auxiliar para cargar categorías
        private async Task CargarCategorias()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
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
                else
                {
                    // Si falla, usar valores predeterminados
                    ViewBag.Categorias = new List<SelectListItem>
                    {
                        new SelectListItem { Value = "1", Text = "Estándar" },
                        new SelectListItem { Value = "2", Text = "Suite" }
                    };
                }
            }
            catch
            {
                // Si falla completamente, usar valores predeterminados
                ViewBag.Categorias = new List<SelectListItem>
                {
                    new SelectListItem { Value = "1", Text = "Estándar" },
                    new SelectListItem { Value = "2", Text = "Suite" }
                };
            }
        }
    }
}