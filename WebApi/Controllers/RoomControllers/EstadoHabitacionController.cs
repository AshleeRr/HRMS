using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using WebApi.Models;
using WebApi.Models.RoomModels;

namespace WebApi.Controllers.RoomControllers
{
    public class EstadoHabitacionController : Controller
    {
        private const string _apiBaseUrl = "https://localhost:7175/api";

        // GET: EstadoHabitacionController
        public async Task<IActionResult> Index()
        {
            List<EstadoHabitacionModel> estados = new List<EstadoHabitacionModel>();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_apiBaseUrl}/EstadoHabitacion/GetEstadoHabitaciones");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        
                        try 
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                estados = JsonConvert.DeserializeObject<List<EstadoHabitacionModel>>(
                                    JsonConvert.SerializeObject(operationResult.Data));
                            }
                        }
                        catch (JsonSerializationException)
                        {
                            try
                            {
                                estados = JsonConvert.DeserializeObject<List<EstadoHabitacionModel>>(content);
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
                            TempData["Error"] = operationResult?.Message ?? $"Error al obtener estados: {response.ReasonPhrase}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al obtener estados: {response.ReasonPhrase}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
            }
            
            TempData["Success"] = TempData["Success"]; 
            return View(estados);
        }

        // GET: EstadoHabitacionController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            EstadoHabitacionModel estado = new EstadoHabitacionModel();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_apiBaseUrl}/EstadoHabitacion/GetEstadoBy(id){id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                estado = JsonConvert.DeserializeObject<EstadoHabitacionModel>(
                                    JsonConvert.SerializeObject(operationResult.Data));
                            }
                        }
                        catch
                        {
                            estado = JsonConvert.DeserializeObject<EstadoHabitacionModel>(content);
                        }
                        
                        return View(estado);
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? $"Error al obtener los detalles del estado: {response.ReasonPhrase}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al obtener los detalles del estado: {response.ReasonPhrase}";
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

        // GET: EstadoHabitacionController/Create
        public ActionResult Create()
        {
            return View(new EstadoHabitacionModel());
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
                    using (var client = new HttpClient())
                    {
                        var json = JsonConvert.SerializeObject(estado);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var response = await client.PostAsync($"{_apiBaseUrl}/EstadoHabitacion/CreateEstadoHabitacion", content);

                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            try
                            {
                                var operationResult = JsonConvert.DeserializeObject<OperationResult>(responseContent);
                                if (operationResult != null && operationResult.IsSuccess)
                                {
                                    TempData["Success"] = operationResult.Message ?? "Estado de habitación creado correctamente.";
                                    return RedirectToAction(nameof(Index));
                                }
                            }
                            catch
                            {
                                TempData["Success"] = "Estado de habitación creado correctamente.";
                                return RedirectToAction(nameof(Index));
                            }
                        }
                        
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? $"Error al crear el estado: {errorContent}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al crear el estado: {errorContent}";
                        }
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
            EstadoHabitacionModel estado = new EstadoHabitacionModel();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_apiBaseUrl}/EstadoHabitacion/GetEstadoBy(id){id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                estado = JsonConvert.DeserializeObject<EstadoHabitacionModel>(
                                    JsonConvert.SerializeObject(operationResult.Data));
                            }
                        }
                        catch
                        {
                            estado = JsonConvert.DeserializeObject<EstadoHabitacionModel>(content);
                        }
                        
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
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? $"Error al obtener el estado: {response.ReasonPhrase}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al obtener el estado: {response.ReasonPhrase}";
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
                    
                    using (var client = new HttpClient())
                    {
                        var json = JsonConvert.SerializeObject(estado);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"{_apiBaseUrl}/EstadoHabitacion/UpdateEstadoHabitacionById{id}");
                        request.Content = content;
                        
                        var response = await client.SendAsync(request);

                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            try
                            {
                                var operationResult = JsonConvert.DeserializeObject<OperationResult>(responseContent);
                                if (operationResult != null && operationResult.IsSuccess)
                                {
                                    TempData["Success"] = operationResult.Message ?? "Estado de habitación actualizado correctamente.";
                                    return RedirectToAction(nameof(Index));
                                }
                            }
                            catch
                            {
                                TempData["Success"] = "Estado de habitación actualizado correctamente.";
                                return RedirectToAction(nameof(Index));
                            }
                        }
                        
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? $"Error al actualizar el estado: {errorContent}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al actualizar el estado: {errorContent}";
                        }
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
            EstadoHabitacionModel estado = new EstadoHabitacionModel();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_apiBaseUrl}/EstadoHabitacion/GetEstadoBy(id){id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                estado = JsonConvert.DeserializeObject<EstadoHabitacionModel>(
                                    JsonConvert.SerializeObject(operationResult.Data));
                            }
                        }
                        catch
                        {
                            estado = JsonConvert.DeserializeObject<EstadoHabitacionModel>(content);
                        }
                        
                        return View(estado);
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? $"Error al obtener el estado: {response.ReasonPhrase}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al obtener el estado: {response.ReasonPhrase}";
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

        // POST: EstadoHabitacionController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.DeleteAsync($"{_apiBaseUrl}/EstadoHabitacion/DeleteEstadoHabitacionById{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(responseContent);
                            if (operationResult != null && operationResult.IsSuccess)
                            {
                                TempData["Success"] = operationResult.Message ?? "Estado de habitación eliminado correctamente.";
                                return RedirectToAction(nameof(Index));
                            }
                        }
                        catch
                        {
                            TempData["Success"] = "Estado de habitación eliminado correctamente.";
                            return RedirectToAction(nameof(Index));
                        }
                    }
                    
                    var errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                        
                        if (operationResult?.Message?.Contains("habitaciones asociadas") == true)
                        {
                            TempData["Error"] = "No se puede eliminar el estado porque tiene habitaciones asociadas.";
                        }
                        else
                        {
                            TempData["Error"] = operationResult?.Message ?? "Error al eliminar el estado.";
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
                }
                
                return RedirectToAction(nameof(Index));
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
            List<EstadoHabitacionModel> estados = new List<EstadoHabitacionModel>();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_apiBaseUrl}/EstadoHabitacion/GetEstadoBy(descripcion){descripcion}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        EstadoHabitacionModel estado = null;
                        
                        try 
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                estado = JsonConvert.DeserializeObject<EstadoHabitacionModel>(
                                    JsonConvert.SerializeObject(operationResult.Data));
                            }
                        }
                        catch (JsonSerializationException)
                        {
                            try
                            {
                                estado = JsonConvert.DeserializeObject<EstadoHabitacionModel>(content);
                            }
                            catch (Exception ex)
                            {
                                TempData["Error"] = $"Error al procesar datos: {ex.Message}";
                                return RedirectToAction(nameof(Index));
                            }
                        }
                        
                        if (estado != null)
                        {
                            estados.Add(estado);
                            ViewBag.TituloLista = $"Estado con descripción: {descripcion}";
                            return View("Index", estados);
                        }
                        
                        TempData["Error"] = "No se encontró el estado.";
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? $"Error al buscar estado: {response.ReasonPhrase}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al buscar estado: {response.ReasonPhrase}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}