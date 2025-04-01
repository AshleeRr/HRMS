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
        private const string _apiBaseUrl = "https://localhost:7175/api";

        // GET: HabitacionController
        public async Task<IActionResult> Index()
        {
            List<HabitacionModel> habitaciones = new List<HabitacionModel>();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_apiBaseUrl}/Habitacion/GetAllHabitaciones");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        
                        try 
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                habitaciones = JsonConvert.DeserializeObject<List<HabitacionModel>>(
                                    JsonConvert.SerializeObject(operationResult.Data));
                            }
                        }
                        catch (JsonSerializationException)
                        {
                            try
                            {
                                habitaciones = JsonConvert.DeserializeObject<List<HabitacionModel>>(content);
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
                            TempData["Error"] = operationResult?.Message ?? $"Error al obtener habitaciones: {response.ReasonPhrase}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al obtener habitaciones: {response.ReasonPhrase}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
            }
            
            TempData["Success"] = TempData["Success"]; 
            return View(habitaciones);
        }

        // GET: HabitacionController/InfoHabitaciones
        public async Task<IActionResult> InfoHabitaciones()
        {
            List<HabitacionInfoModel> habitacionesInfo = new List<HabitacionInfoModel>();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_apiBaseUrl}/Habitacion/GetInfoHabitaciones");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        
                        try 
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                habitacionesInfo = JsonConvert.DeserializeObject<List<HabitacionInfoModel>>(
                                    JsonConvert.SerializeObject(operationResult.Data));
                            }
                        }
                        catch (JsonSerializationException)
                        {
                            try
                            {
                                habitacionesInfo = JsonConvert.DeserializeObject<List<HabitacionInfoModel>>(content);
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
                            TempData["Error"] = operationResult?.Message ?? $"Error al obtener información detallada: {response.ReasonPhrase}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al obtener información detallada: {response.ReasonPhrase}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
            }
            
            return View(habitacionesInfo);
        }

        // GET: HabitacionController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            HabitacionModel habitacion = new HabitacionModel();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_apiBaseUrl}/Habitacion/GetByHabitacionById{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                habitacion = JsonConvert.DeserializeObject<HabitacionModel>(
                                    JsonConvert.SerializeObject(operationResult.Data));
                            }
                        }
                        catch
                        {
                            habitacion = JsonConvert.DeserializeObject<HabitacionModel>(content);
                        }
                        
                        return View(habitacion);
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? $"Error al obtener detalles de la habitación: {response.ReasonPhrase}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al obtener detalles de la habitación: {response.ReasonPhrase}";
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
                    using (var client = new HttpClient())
                    {
                        var json = JsonConvert.SerializeObject(habitacion);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var response = await client.PostAsync($"{_apiBaseUrl}/Habitacion/CreateHabitacion", content);

                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            try
                            {
                                var operationResult = JsonConvert.DeserializeObject<OperationResult>(responseContent);
                                if (operationResult != null && operationResult.IsSuccess)
                                {
                                    TempData["Success"] = operationResult.Message ?? "Habitación creada correctamente.";
                                    return RedirectToAction(nameof(Index));
                                }
                            }
                            catch
                            {
                                TempData["Success"] = "Habitación creada correctamente.";
                                return RedirectToAction(nameof(Index));
                            }
                        }
                        
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? $"Error al crear habitación: {errorContent}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al crear habitación: {errorContent}";
                        }
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
            HabitacionModel habitacion = new HabitacionModel();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_apiBaseUrl}/Habitacion/GetByHabitacionById{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                habitacion = JsonConvert.DeserializeObject<HabitacionModel>(
                                    JsonConvert.SerializeObject(operationResult.Data));
                            }
                        }
                        catch
                        {
                            habitacion = JsonConvert.DeserializeObject<HabitacionModel>(content);
                        }
                        
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
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? $"Error al obtener la habitación: {response.ReasonPhrase}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al obtener la habitación: {response.ReasonPhrase}";
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
                    
                    using (var client = new HttpClient())
                    {
                        var json = JsonConvert.SerializeObject(habitacion);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var response = await client.PutAsync($"{_apiBaseUrl}/Habitacion/(UpdateHabitacionBy){id}", content);

                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            try
                            {
                                var operationResult = JsonConvert.DeserializeObject<OperationResult>(responseContent);
                                if (operationResult != null && operationResult.IsSuccess)
                                {
                                    TempData["Success"] = operationResult.Message ?? "Habitación actualizada correctamente.";
                                    return RedirectToAction(nameof(Index));
                                }
                            }
                            catch
                            {
                                TempData["Success"] = "Habitación actualizada correctamente.";
                                return RedirectToAction(nameof(Index));
                            }
                        }
                        
                        var errorContent = await response.Content.ReadAsStringAsync();
                        
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? "Error al actualizar la habitación.";
                        }
                        catch
                        {
                            TempData["Error"] = "No se puede actualizar la habitación en este momento.";
                        }
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
            HabitacionModel habitacion = new HabitacionModel();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_apiBaseUrl}/Habitacion/GetByHabitacionById{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                habitacion = JsonConvert.DeserializeObject<HabitacionModel>(
                                    JsonConvert.SerializeObject(operationResult.Data));
                            }
                        }
                        catch
                        {
                            habitacion = JsonConvert.DeserializeObject<HabitacionModel>(content);
                        }
                        
                        return View(habitacion);
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? $"Error al obtener la habitación: {response.ReasonPhrase}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al obtener la habitación: {response.ReasonPhrase}";
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

        // POST: HabitacionController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.DeleteAsync($"{_apiBaseUrl}/Habitacion/DeleteHabitacionBy{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(responseContent);
                            if (operationResult != null && operationResult.IsSuccess)
                            {
                                TempData["Success"] = operationResult.Message ?? "Habitación eliminada correctamente.";
                                return RedirectToAction(nameof(Index));
                            }
                        }
                        catch
                        {
                            TempData["Success"] = "Habitación eliminada correctamente.";
                            return RedirectToAction(nameof(Index));
                        }
                    }
                    
                    var errorContent = await response.Content.ReadAsStringAsync();
                    
                    try
                    {
                        var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                        
                        if (operationResult?.Message?.Contains("reservas activas") == true)
                        {
                            TempData["Error"] = "No se puede eliminar la habitación porque tiene reservas activas.";
                        }
                        else
                        {
                            TempData["Error"] = operationResult?.Message ?? "Error al eliminar la habitación.";
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
            List<HabitacionModel> habitaciones = new List<HabitacionModel>();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_apiBaseUrl}/Habitacion/GetHabitacionByPiso/{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                habitaciones = JsonConvert.DeserializeObject<List<HabitacionModel>>(
                                    JsonConvert.SerializeObject(operationResult.Data));
                            }
                        }
                        catch
                        {
                            habitaciones = JsonConvert.DeserializeObject<List<HabitacionModel>>(content);
                        }
                        
                        ViewBag.TituloLista = $"Habitaciones en el Piso {id}";
                        return View("Filtered", habitaciones);
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? $"Error al filtrar habitaciones: {response.ReasonPhrase}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al filtrar habitaciones: {response.ReasonPhrase}";
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

        // GET: HabitacionController/FilterByCategoria/1
        public async Task<IActionResult> FilterByCategoria(string categoria)
        {
            List<HabitacionModel> habitaciones = new List<HabitacionModel>();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_apiBaseUrl}/Habitacion/GetHabitacionByCategoria/{categoria}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                habitaciones = JsonConvert.DeserializeObject<List<HabitacionModel>>(
                                    JsonConvert.SerializeObject(operationResult.Data));
                            }
                        }
                        catch
                        {
                            habitaciones = JsonConvert.DeserializeObject<List<HabitacionModel>>(content);
                        }
                        
                        ViewBag.TituloLista = $"Habitaciones de Categoría: {categoria}";
                        return View("Filtered", habitaciones);
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? $"Error al filtrar habitaciones: {response.ReasonPhrase}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al filtrar habitaciones: {response.ReasonPhrase}";
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

        // GET: HabitacionController/FilterByNumero/101
        public async Task<IActionResult> FilterByNumero(string numero)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_apiBaseUrl}/Habitacion/GetHabitacionBy/{numero}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        HabitacionModel habitacion = null;
                        
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                habitacion = JsonConvert.DeserializeObject<HabitacionModel>(
                                    JsonConvert.SerializeObject(operationResult.Data));
                            }
                        }
                        catch
                        {
                            habitacion = JsonConvert.DeserializeObject<HabitacionModel>(content);
                        }
                        
                        if (habitacion != null)
                        {
                            var habitaciones = new List<HabitacionModel> { habitacion };
                            ViewBag.TituloLista = $"Habitación Número: {numero}";
                            return View("Filtered", habitaciones);
                        }
                        else
                        {
                            TempData["Error"] = "No se encontró la habitación.";
                            return RedirectToAction(nameof(Index));
                        }
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? $"Error al buscar habitación: {response.ReasonPhrase}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al buscar habitación: {response.ReasonPhrase}";
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

        private async Task CargarListasDesplegables()
        {
            // Cargar estados de habitación
            try
            {
                using (var client = new HttpClient())
                {
                    var responseEstados = await client.GetAsync($"{_apiBaseUrl}/EstadoHabitacion/GetEstadoHabitaciones");
                    if (responseEstados.IsSuccessStatusCode)
                    {
                        var contentEstados = await responseEstados.Content.ReadAsStringAsync();
                        List<EstadoHabitacionModel> estados = null;
                        
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(contentEstados);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                estados = JsonConvert.DeserializeObject<List<EstadoHabitacionModel>>(
                                    JsonConvert.SerializeObject(operationResult.Data));
                            }
                        }
                        catch
                        {
                            estados = JsonConvert.DeserializeObject<List<EstadoHabitacionModel>>(contentEstados);
                        }
                        
                        if (estados != null && estados.Any())
                        {
                            ViewBag.EstadosHabitacion = estados.Select(e => new SelectListItem
                            {
                                Value = e.IdEstadoHabitacion.ToString(),
                                Text = e.Descripcion
                            }).ToList();
                        }
                        else
                        {
                            ViewBag.EstadosHabitacion = null;
                        }
                    }
                }
            }
            catch (Exception)
            {
                ViewBag.EstadosHabitacion = null;
            }

            // Cargar pisos
            try
            {
                using (var client = new HttpClient())
                {
                    var responsePisos = await client.GetAsync($"{_apiBaseUrl}/Piso/GetAllPisos");
                    if (responsePisos.IsSuccessStatusCode)
                    {
                        var contentPisos = await responsePisos.Content.ReadAsStringAsync();
                        List<PisoModel> pisos = null;
                        
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(contentPisos);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                pisos = JsonConvert.DeserializeObject<List<PisoModel>>(
                                    JsonConvert.SerializeObject(operationResult.Data));
                            }
                        }
                        catch
                        {
                            pisos = JsonConvert.DeserializeObject<List<PisoModel>>(contentPisos);
                        }
                        
                        if (pisos != null && pisos.Any())
                        {
                            ViewBag.Pisos = pisos.Select(p => new SelectListItem
                            {
                                Value = p.IdPiso.ToString(),
                                Text = p.Descripcion
                            }).ToList();
                        }
                        else
                        {
                            ViewBag.Pisos = null;
                        }
                    }
                }
            }
            catch (Exception)
            {
                ViewBag.Pisos = null;
            }

            // Cargar categorías
            try
            {
                using (var client = new HttpClient())
                {
                    var responseCategorias = await client.GetAsync($"{_apiBaseUrl}/Categoria/GetAllCategorias");
                    if (responseCategorias.IsSuccessStatusCode)
                    {
                        var contentCategorias = await responseCategorias.Content.ReadAsStringAsync();
                        List<CategoriaModel> categorias = null;
                        
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(contentCategorias);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                categorias = JsonConvert.DeserializeObject<List<CategoriaModel>>(
                                    JsonConvert.SerializeObject(operationResult.Data));
                            }
                        }
                        catch
                        {
                            categorias = JsonConvert.DeserializeObject<List<CategoriaModel>>(contentCategorias);
                        }
                        
                        if (categorias != null && categorias.Any())
                        {
                            ViewBag.Categorias = categorias.Select(c => new SelectListItem
                            {
                                Value = c.IdCategoria.ToString(),
                                Text = c.Descripcion
                            }).ToList();
                        }
                        else
                        {
                            ViewBag.Categorias = null;
                        }
                    }
                }
            }
            catch (Exception)
            {
                ViewBag.Categorias = null;
            }
        }
    }
}