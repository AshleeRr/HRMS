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
        private const string _apiBaseUrl = "https://localhost:7175/api";

        // GET: TarifaController
        public async Task<ActionResult> Index()
        {
            List<TarifaModel> tarifas = new List<TarifaModel>();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_apiBaseUrl}/Tarifa/GetAllTarifas");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        
                        try 
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                tarifas = JsonConvert.DeserializeObject<List<TarifaModel>>(
                                    JsonConvert.SerializeObject(operationResult.Data));
                            }
                        }
                        catch (JsonSerializationException)
                        {
                            try
                            {
                                tarifas = JsonConvert.DeserializeObject<List<TarifaModel>>(content);
                            }
                            catch (Exception ex)
                            {
                                TempData["Error"] = $"Error al procesar datos: {ex.Message}";
                            }
                        }
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? $"Error al obtener tarifas: {response.ReasonPhrase}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al obtener tarifas: {response.ReasonPhrase}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
            }
            
            TempData["Success"] = TempData["Success"]; 
            return View(tarifas);
        }

        // GET: TarifaController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            TarifaModel tarifa = new TarifaModel();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_apiBaseUrl}/Tarifa/GetTarifaById{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                tarifa = JsonConvert.DeserializeObject<TarifaModel>(
                                    JsonConvert.SerializeObject(operationResult.Data));
                            }
                        }
                        catch
                        {
                            tarifa = JsonConvert.DeserializeObject<TarifaModel>(content);
                        }
                        
                        await CargarCategorias();
                        return View(tarifa);
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? $"Error al obtener los detalles de la tarifa: {response.ReasonPhrase}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al obtener los detalles de la tarifa: {response.ReasonPhrase}";
                        }
                        return RedirectToAction(nameof(Index));
                    }
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
            return View(new TarifaModel());
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

                    using (var client = new HttpClient())
                    {
                        var json = JsonConvert.SerializeObject(tarifa);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var response = await client.PostAsync($"{_apiBaseUrl}/Tarifa/CreateTarifa", content);

                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            try
                            {
                                var operationResult = JsonConvert.DeserializeObject<OperationResult>(responseContent);
                                if (operationResult != null && operationResult.IsSuccess)
                                {
                                    TempData["Success"] = operationResult.Message ?? "Tarifa creada correctamente.";
                                    return RedirectToAction(nameof(Index));
                                }
                            }
                            catch
                            {
                                TempData["Success"] = "Tarifa creada correctamente.";
                                return RedirectToAction(nameof(Index));
                            }
                        }
                        
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? $"Error al crear la tarifa: {errorContent}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al crear la tarifa: {errorContent}";
                        }
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
            TarifaModel tarifa = new TarifaModel();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_apiBaseUrl}/Tarifa/GetTarifaById{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                tarifa = JsonConvert.DeserializeObject<TarifaModel>(
                                    JsonConvert.SerializeObject(operationResult.Data));
                            }
                        }
                        catch
                        {
                            tarifa = JsonConvert.DeserializeObject<TarifaModel>(content);
                        }
                        
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
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? $"Error al obtener la tarifa: {response.ReasonPhrase}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al obtener la tarifa: {response.ReasonPhrase}";
                        }
                        return RedirectToAction(nameof(Index));
                    }
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

                    if (tarifa.IdTarifa != id)
                    {
                        tarifa.IdTarifa = id;
                    }
                    
                    tarifa.ChangeTime = DateTime.Now;
                    
                    using (var client = new HttpClient())
                    {
                        var json = JsonConvert.SerializeObject(tarifa);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var response = await client.PutAsync($"{_apiBaseUrl}/Tarifa/UpdateTarifaBy{id}", content);

                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            try
                            {
                                var operationResult = JsonConvert.DeserializeObject<OperationResult>(responseContent);
                                if (operationResult != null && operationResult.IsSuccess)
                                {
                                    TempData["Success"] = operationResult.Message ?? "Tarifa actualizada correctamente.";
                                    return RedirectToAction(nameof(Index));
                                }
                            }
                            catch
                            {
                                TempData["Success"] = "Tarifa actualizada correctamente.";
                                return RedirectToAction(nameof(Index));
                            }
                        }
                        
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? $"Error al actualizar la tarifa: {errorContent}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al actualizar la tarifa: {errorContent}";
                        }
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
            TarifaModel tarifa = new TarifaModel();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_apiBaseUrl}/Tarifa/GetTarifaById{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                tarifa = JsonConvert.DeserializeObject<TarifaModel>(
                                    JsonConvert.SerializeObject(operationResult.Data));
                            }
                        }
                        catch
                        {
                            tarifa = JsonConvert.DeserializeObject<TarifaModel>(content);
                        }
                        
                        await CargarCategorias();
                        return View(tarifa);
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? $"Error al obtener la tarifa: {response.ReasonPhrase}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al obtener la tarifa: {response.ReasonPhrase}";
                        }
                        return RedirectToAction(nameof(Index));
                    }
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
                using (var client = new HttpClient())
                {
                    var response = await client.DeleteAsync($"{_apiBaseUrl}/Tarifa/DeleteTarifaBy{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(responseContent);
                            if (operationResult != null && operationResult.IsSuccess)
                            {
                                TempData["Success"] = operationResult.Message ?? "Tarifa eliminada correctamente.";
                                return RedirectToAction(nameof(Index));
                            }
                        }
                        catch
                        {
                            TempData["Success"] = "Tarifa eliminada correctamente.";
                            return RedirectToAction(nameof(Index));
                        }
                    }
                    
                    var errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                        
                        if (operationResult?.Message?.Contains("reservas asociadas") == true)
                        {
                            TempData["Error"] = "No se puede eliminar la tarifa porque tiene reservas asociadas.";
                        }
                        else
                        {
                            TempData["Error"] = operationResult?.Message ?? "Error al eliminar la tarifa.";
                        }
                    }
                    catch
                    {
                        TempData["Error"] = "No se puede eliminar la tarifa en este momento.";
                    }
                    
                    if (Request.Path.Value?.Contains("/Delete/") == true)
                    {
                        return RedirectToAction(nameof(Delete), new { id });
                    }
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
        public async Task<ActionResult> GetByFecha(DateTime fecha)
        {
            List<TarifaModel> tarifas = new List<TarifaModel>();
            try
            {
                var fechaFormateada = fecha.ToString("yyyy-MM-dd");
                
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_apiBaseUrl}/Tarifa/GetTarifaByFecha?fecha={fechaFormateada}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                tarifas = JsonConvert.DeserializeObject<List<TarifaModel>>(
                                    JsonConvert.SerializeObject(operationResult.Data));
                            }
                        }
                        catch
                        {
                            tarifas = JsonConvert.DeserializeObject<List<TarifaModel>>(content);
                        }
                        
                        ViewBag.TituloLista = $"Tarifas para la fecha: {fecha.ToString("dd/MM/yyyy")}";
                        return View("Index", tarifas);
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? $"Error al buscar tarifas: {response.ReasonPhrase}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al buscar tarifas: {response.ReasonPhrase}";
                        }
                        return RedirectToAction(nameof(Index));
                    }
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
            List<TarifaModel> tarifas = new List<TarifaModel>();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_apiBaseUrl}/Tarifa/GetTarifaByPrecio/{precio}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                tarifas = JsonConvert.DeserializeObject<List<TarifaModel>>(
                                    JsonConvert.SerializeObject(operationResult.Data));
                            }
                        }
                        catch
                        {
                            tarifas = JsonConvert.DeserializeObject<List<TarifaModel>>(content);
                        }
                        
                        ViewBag.TituloLista = $"Tarifas con precio: ${precio}";
                        return View("Index", tarifas);
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? $"Error al buscar tarifas: {response.ReasonPhrase}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al buscar tarifas: {response.ReasonPhrase}";
                        }
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task CargarCategorias()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_apiBaseUrl}/Categoria/GetAllCategorias");
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        List<CategoriaModel> categorias = null;
                        
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                categorias = JsonConvert.DeserializeObject<List<CategoriaModel>>(
                                    JsonConvert.SerializeObject(operationResult.Data));
                            }
                        }
                        catch
                        {
                            categorias = JsonConvert.DeserializeObject<List<CategoriaModel>>(content);
                        }
                        
                        if (categorias != null && categorias.Any())
                        {
                            ViewBag.Categorias = categorias.Select(c => new SelectListItem
                            {
                                Value = c.IdCategoria.ToString(),
                                Text = c.Descripcion
                            }).ToList();
                            return;
                        }
                    }
                }
            }
            catch
            {
                
                
            }
            
            ViewBag.Categorias = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Estándar" },
                new SelectListItem { Value = "2", Text = "Suite" }
            };
        }
    }
}