using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using WebApi.Models;
using WebApi.Models.RoomModels;

namespace WebApi.Controllers.RoomControllers
{
    public class PisoController : Controller
    {
        private const string _apiBaseUrl = "https://localhost:7175/api";

        // GET: PisoController
        public async Task<IActionResult> Index()
        {
            List<PisoModel> pisos = new List<PisoModel>();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_apiBaseUrl}/Piso/GetAllPisos");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        
                        try 
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                pisos = JsonConvert.DeserializeObject<List<PisoModel>>(
                                    JsonConvert.SerializeObject(operationResult.Data));
                            }
                        }
                        catch (JsonSerializationException)
                        {
                            try
                            {
                                pisos = JsonConvert.DeserializeObject<List<PisoModel>>(content);
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
                            TempData["Error"] = operationResult?.Message ?? $"Error al obtener pisos: {response.ReasonPhrase}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al obtener pisos: {response.ReasonPhrase}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
            }
            
            TempData["Success"] = TempData["Success"]; // Preservar mensajes
            return View(pisos);
        }

        // GET: PisoController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            PisoModel piso = new PisoModel();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_apiBaseUrl}/Piso/GetPisoById{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                piso = JsonConvert.DeserializeObject<PisoModel>(
                                    JsonConvert.SerializeObject(operationResult.Data));
                            }
                        }
                        catch
                        {
                            piso = JsonConvert.DeserializeObject<PisoModel>(content);
                        }
                        
                        return View(piso);
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? $"Error al obtener los detalles del piso: {response.ReasonPhrase}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al obtener los detalles del piso: {response.ReasonPhrase}";
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

        // GET: PisoController/Create
        public ActionResult Create()
        {
            return View(new PisoModel());
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
                    using (var client = new HttpClient())
                    {
                        var json = JsonConvert.SerializeObject(piso);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var response = await client.PostAsync($"{_apiBaseUrl}/Piso/CreatePiso", content);

                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            try
                            {
                                var operationResult = JsonConvert.DeserializeObject<OperationResult>(responseContent);
                                if (operationResult != null && operationResult.IsSuccess)
                                {
                                    TempData["Success"] = operationResult.Message ?? "Piso creado correctamente.";
                                    return RedirectToAction(nameof(Index));
                                }
                            }
                            catch
                            {
                                TempData["Success"] = "Piso creado correctamente.";
                                return RedirectToAction(nameof(Index));
                            }
                        }
                        
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? $"Error al crear el piso: {errorContent}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al crear el piso: {errorContent}";
                        }
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
            PisoModel piso = new PisoModel();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_apiBaseUrl}/Piso/GetPisoById{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                piso = JsonConvert.DeserializeObject<PisoModel>(
                                    JsonConvert.SerializeObject(operationResult.Data));
                            }
                        }
                        catch
                        {
                            piso = JsonConvert.DeserializeObject<PisoModel>(content);
                        }
                        
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
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? $"Error al obtener el piso: {response.ReasonPhrase}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al obtener el piso: {response.ReasonPhrase}";
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
                    
                    using (var client = new HttpClient())
                    {
                        var json = JsonConvert.SerializeObject(piso);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"{_apiBaseUrl}/Piso/UpdatePiso{id}");
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
                                    TempData["Success"] = operationResult.Message ?? "Piso actualizado correctamente.";
                                    return RedirectToAction(nameof(Index));
                                }
                            }
                            catch
                            {
                                TempData["Success"] = "Piso actualizado correctamente.";
                                return RedirectToAction(nameof(Index));
                            }
                        }
                        
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? $"Error al actualizar el piso: {errorContent}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al actualizar el piso: {errorContent}";
                        }
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
            PisoModel piso = new PisoModel();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_apiBaseUrl}/Piso/GetPisoById{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                piso = JsonConvert.DeserializeObject<PisoModel>(
                                    JsonConvert.SerializeObject(operationResult.Data));
                            }
                        }
                        catch
                        {
                            piso = JsonConvert.DeserializeObject<PisoModel>(content);
                        }
                        
                        return View(piso);
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? $"Error al obtener el piso: {response.ReasonPhrase}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al obtener el piso: {response.ReasonPhrase}";
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

        // POST: PisoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.DeleteAsync($"{_apiBaseUrl}/Piso/DeletePiso{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(responseContent);
                            if (operationResult != null && operationResult.IsSuccess)
                            {
                                TempData["Success"] = operationResult.Message ?? "Piso eliminado correctamente.";
                                return RedirectToAction(nameof(Index));
                            }
                        }
                        catch
                        {
                            TempData["Success"] = "Piso eliminado correctamente.";
                            return RedirectToAction(nameof(Index));
                        }
                    }
                    
                    var errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                        
                        if (operationResult?.Message?.Contains("habitaciones asociadas") == true || 
                            operationResult?.Message?.Contains("relacionado con habitaciones") == true)
                        {
                            TempData["Error"] = "No se puede eliminar el piso porque tiene habitaciones asociadas.";
                        }
                        else
                        {
                            TempData["Error"] = operationResult?.Message ?? "Error al eliminar el piso.";
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
                }
                
                return RedirectToAction(nameof(Index));
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
            List<PisoModel> pisos = new List<PisoModel>();
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_apiBaseUrl}/Piso/GetPisoByDescripcion{descripcion}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        PisoModel piso = null;
                        
                        try 
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                            if (operationResult?.IsSuccess == true && operationResult.Data != null)
                            {
                                piso = JsonConvert.DeserializeObject<PisoModel>(
                                    JsonConvert.SerializeObject(operationResult.Data));
                            }
                        }
                        catch (JsonSerializationException)
                        {
                            try
                            {
                                piso = JsonConvert.DeserializeObject<PisoModel>(content);
                            }
                            catch (Exception ex)
                            {
                                TempData["Error"] = $"Error al procesar datos: {ex.Message}";
                                return RedirectToAction(nameof(Index));
                            }
                        }
                        
                        if (piso != null)
                        {
                            pisos.Add(piso);
                            ViewBag.TituloLista = $"Piso con descripción: {descripcion}";
                            return View("Index", pisos);
                        }
                        
                        TempData["Error"] = "No se encontró el piso.";
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var operationResult = JsonConvert.DeserializeObject<OperationResult>(errorContent);
                            TempData["Error"] = operationResult?.Message ?? $"Error al buscar piso: {response.ReasonPhrase}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al buscar piso: {response.ReasonPhrase}";
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