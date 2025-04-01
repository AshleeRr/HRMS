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
        private const string _apiBaseUrl = "https://localhost:7175/api";

        // GET: CategoriaController
        public async Task<ActionResult> Index()
        {
            List<CategoriaModel> categorias = new List<CategoriaModel>();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_apiBaseUrl}/Categoria/GetAllCategorias");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        
                        try 
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                categorias = JsonConvert.DeserializeObject<List<CategoriaModel>>(
                                    JsonConvert.SerializeObject(operationResult.Data));
                            }
                        }
                        catch (JsonSerializationException)
                        {
                            try
                            {
                                categorias = JsonConvert.DeserializeObject<List<CategoriaModel>>(content);
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
                            TempData["Error"] = operationResult?.Message ?? $"Error al obtener categorías: {response.ReasonPhrase}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al obtener categorías: {response.ReasonPhrase}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
            }
            
            return View(categorias);
        }

        // GET: CategoriaController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            CategoriaModel categoria = new CategoriaModel();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_apiBaseUrl}/Categoria/GetCategoriaById{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                categoria = JsonConvert.DeserializeObject<CategoriaModel>(
                                    JsonConvert.SerializeObject(operationResult.Data));
                            }
                        }
                        catch
                        {
                            categoria = JsonConvert.DeserializeObject<CategoriaModel>(content);
                        }
                        
                        await CargarServicios();
                        return View(categoria);
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? $"Error al obtener los detalles de la categoría: {response.ReasonPhrase}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al obtener los detalles de la categoría: {response.ReasonPhrase}";
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

        // GET: CategoriaController/Create
        public async Task<ActionResult> Create()
        {
            await CargarServicios();
            return View(new CategoriaModel());
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
                    using (var client = new HttpClient())
                    {
                        var json = JsonConvert.SerializeObject(categoria);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var response = await client.PostAsync($"{_apiBaseUrl}/Categoria/CreateCategoria", content);

                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(responseContent);
                            
                            if (operationResult != null && operationResult.IsSuccess)
                            {
                                TempData["Success"] = operationResult.Message ?? "Categoría creada correctamente.";
                                return RedirectToAction(nameof(Index));
                            }
                        }
                        
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? $"Error al crear la categoría: {errorContent}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al crear la categoría: {errorContent}";
                        }
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
            CategoriaModel categoria = new CategoriaModel();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_apiBaseUrl}/Categoria/GetCategoriaById{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                categoria = JsonConvert.DeserializeObject<CategoriaModel>(
                                    JsonConvert.SerializeObject(operationResult.Data));
                            }
                        }
                        catch
                        {
                            categoria = JsonConvert.DeserializeObject<CategoriaModel>(content);
                        }
                        
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
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? $"Error al obtener la categoría: {response.ReasonPhrase}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al obtener la categoría: {response.ReasonPhrase}";
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
                    
                    using (var client = new HttpClient())
                    {
                        var json = JsonConvert.SerializeObject(categoria);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var response = await client.PutAsync($"{_apiBaseUrl}/Categoria/UpdateCategoriaById{id}", content);

                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(responseContent);
                            
                            if (operationResult != null && operationResult.IsSuccess)
                            {
                                TempData["Success"] = operationResult.Message ?? "Categoría actualizada correctamente.";
                                return RedirectToAction(nameof(Index));
                            }
                        }
                        
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? $"Error al actualizar la categoría: {errorContent}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al actualizar la categoría: {errorContent}";
                        }
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
            CategoriaModel categoria = new CategoriaModel();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_apiBaseUrl}/Categoria/GetCategoriaById{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                categoria = JsonConvert.DeserializeObject<CategoriaModel>(
                                    JsonConvert.SerializeObject(operationResult.Data));
                            }
                        }
                        catch
                        {
                            categoria = JsonConvert.DeserializeObject<CategoriaModel>(content);
                        }
                        
                        await CargarServicios();
                        return View(categoria);
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? $"Error al obtener la categoría: {response.ReasonPhrase}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al obtener la categoría: {response.ReasonPhrase}";
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

        // POST: CategoriaController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.DeleteAsync($"{_apiBaseUrl}/Categoria/DeleteCategoriaById{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var operationResult = JsonConvert.DeserializeObject<OperationResult>(responseContent);
                        
                        TempData["Success"] = operationResult?.Message ?? "Categoría eliminada correctamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            
                            if (operationResult?.Message?.Contains("habitaciones asociadas") == true)
                            {
                                TempData["Error"] = "No se puede eliminar la categoría porque tiene habitaciones asociadas.";
                            }
                            else
                            {
                                TempData["Error"] = operationResult?.Message ?? "Error al eliminar la categoría.";
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
            List<CategoriaModel> categorias = new List<CategoriaModel>();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_apiBaseUrl}/Categoria/GetCategoriaByDescripcion/{descripcion}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        CategoriaModel categoria = null;
                        
                        try 
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                categoria = JsonConvert.DeserializeObject<CategoriaModel>(
                                    JsonConvert.SerializeObject(operationResult.Data));
                            }
                            else if (operationResult != null)
                            {
                                TempData["Error"] = operationResult.Message ?? "No se encontró la categoría.";
                                return RedirectToAction(nameof(Index));
                            }
                        }
                        catch (JsonSerializationException)
                        {
                            try
                            {
                                categoria = JsonConvert.DeserializeObject<CategoriaModel>(content);
                            }
                            catch (Exception ex)
                            {
                                TempData["Error"] = $"Error al procesar datos: {ex.Message}";
                                return RedirectToAction(nameof(Index));
                            }
                        }
                        
                        if (categoria != null)
                        {
                            categorias.Add(categoria);
                            ViewBag.TituloLista = $"Categoría con descripción: {descripcion}";
                            return View("Index", categorias);
                        }
                        
                        TempData["Error"] = "No se encontró la categoría.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? $"Error al buscar categoría: {response.ReasonPhrase}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al buscar categoría: {response.ReasonPhrase}";
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

        // GET: CategoriaController/GetByCapacidad
        public async Task<ActionResult> GetByCapacidad(int capacidad)
        {
            List<CategoriaModel> categorias = new List<CategoriaModel>();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_apiBaseUrl}/Categoria/GetHabitacionByCapacidad/{capacidad}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        
                        try 
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                categorias = JsonConvert.DeserializeObject<List<CategoriaModel>>(
                                    JsonConvert.SerializeObject(operationResult.Data));
                            }
                            else if (operationResult != null)
                            {
                                TempData["Error"] = operationResult.Message ?? "No se encontraron categorías con esa capacidad.";
                                return RedirectToAction(nameof(Index));
                            }
                        }
                        catch (JsonSerializationException)
                        {
                            try
                            {
                                categorias = JsonConvert.DeserializeObject<List<CategoriaModel>>(content);
                            }
                            catch (Exception ex)
                            {
                                TempData["Error"] = $"Error al procesar datos: {ex.Message}";
                                return RedirectToAction(nameof(Index));
                            }
                        }
                        
                        if (categorias.Any())
                        {
                            ViewBag.TituloLista = $"Categorías por capacidad: {capacidad}";
                            return View("Index", categorias);
                        }
                        
                        TempData["Error"] = "No se encontraron categorías con esa capacidad.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? $"Error al buscar categorías: {response.ReasonPhrase}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al buscar categorías: {response.ReasonPhrase}";
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

        // Método auxiliar para cargar servicios
        private async Task CargarServicios()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_apiBaseUrl}/Servicio/GetAllServicios");
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                List<ServicioModel> servicios;
                                
                                if (operationResult.Data is List<ServicioModel> directList)
                                {
                                    servicios = directList;
                                }
                                else
                                {
                                    var json = JsonConvert.SerializeObject(operationResult.Data);
                                    servicios = JsonConvert.DeserializeObject<List<ServicioModel>>(json);
                                }
                                
                                if (servicios != null && servicios.Any())
                                {
                                    var selectItems = new List<SelectListItem>();
                                    
                                    foreach (var s in servicios)
                                    {
                                        selectItems.Add(new SelectListItem
                                        {
                                            Value = s.IdServicio.ToString(),
                                            Text = s.Nombre
                                        });
                                    }
                                    
                                    ViewBag.Servicios = selectItems;
                                    return;
                                }
                            }
                        }
                        catch (JsonSerializationException)
                        {
                            try
                            {
                                var servicios = JsonConvert.DeserializeObject<List<ServicioModel>>(content);
                                
                                if (servicios != null && servicios.Any())
                                {
                                    var selectItems = new List<SelectListItem>();
                                    
                                    foreach (var s in servicios)
                                    {
                                        selectItems.Add(new SelectListItem
                                        {
                                            Value = s.IdServicio.ToString(),
                                            Text = s.Nombre
                                        });
                                    }
                                    
                                    ViewBag.Servicios = selectItems;
                                    return;
                                }
                            }
                            catch
                            {
                                SetDefaultServices();
                            }
                        }
                    }
                    
                    SetDefaultServices();
                }
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