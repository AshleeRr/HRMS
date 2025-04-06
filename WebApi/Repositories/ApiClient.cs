using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WebApi.Interfaces;
using WebApi.Models;

namespace WebApi.Repositories
{
    /// <summary>
    /// Cliente HTTP para consumir la API
    /// </summary>
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ApiClient(string baseUrl)
        {
            _httpClient = new HttpClient();
            _baseUrl = baseUrl;
        }

        /// <summary>
        /// Realiza una petición GET a la API
        /// </summary>
        public async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                // Preservar el formato original del endpoint
                string url = $"{_baseUrl}/{endpoint}";
                
                var response = await _httpClient.GetAsync(url);
                var result = await ProcessResponseAsync<T>(response);

                if (result == null || EqualityComparer<T>.Default.Equals(result, default(T)))
                {
                    if (typeof(T) == typeof(OperationResult))
                    {
                        return (T)(object)OperationResult.Failure($"Error al obtener datos de {endpoint}");
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                if (typeof(T) == typeof(OperationResult))
                {
                    return (T)(object)OperationResult.Failure($"Error inesperado: {ex.Message}");
                }

                return default(T);
            }
        }

        /// <summary>
        /// Realiza una petición GET a la API para obtener un elemento por ID
        /// </summary>
        public async Task<T> GetByIdAsync<T>(string endpoint, int id)
        {
            try
            {
                // No modificar el endpoint - dejarlo como está
                string url = $"{_baseUrl}/{endpoint}{id}";
                
                var response = await _httpClient.GetAsync(url);
                var result = await ProcessResponseAsync<T>(response);

                if (result == null || EqualityComparer<T>.Default.Equals(result, default(T)))
                {
                    if (typeof(T) == typeof(OperationResult))
                    {
                        return (T)(object)OperationResult.Failure($"Error al obtener datos con ID {id} de {endpoint}");
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                if (typeof(T) == typeof(OperationResult))
                {
                    return (T)(object)OperationResult.Failure($"Error inesperado: {ex.Message}");
                }

                return default(T);
            }
        }

        /// <summary>
        /// Realiza una petición POST a la API
        /// </summary>
        public async Task<OperationResult> PostAsync<T>(string endpoint, T data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                string url = $"{_baseUrl}/{endpoint}";
                var response = await _httpClient.PostAsync(url, content);
                
                return await ProcessOperationResultAsync(response);
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al realizar POST: {ex.Message}");
            }
        }

        /// <summary>
        /// Realiza una petición PUT a la API
        /// </summary>
        public async Task<OperationResult> PutAsync<T>(string endpoint, int id, T data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                // Concatenar el ID directamente sin barra diagonal
                string url = $"{_baseUrl}/{endpoint}{id}";
                var response = await _httpClient.PutAsync(url, content);
                
                return await ProcessOperationResultAsync(response);
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al realizar PUT: {ex.Message}");
            }
        }

        /// <summary>
        /// Realiza una petición DELETE a la API
        /// </summary>
        public async Task<OperationResult> DeleteAsync(string endpoint, int id)
        {
            try
            {
                // Concatenar el ID directamente sin barra diagonal
                string url = $"{_baseUrl}/{endpoint}{id}";
                var response = await _httpClient.DeleteAsync(url);
                
                // Manejo específico para errores comunes en eliminación
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode && content.Contains("habitaciones asociadas"))
                {
                    return OperationResult.Failure("No se puede eliminar porque tiene elementos asociados. Debe eliminar o reubicar estos elementos primero.");
                }
                
                return await ProcessOperationResultAsync(response);
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al realizar DELETE: {ex.Message}");
            }
        }

        /// <summary>
        /// Realiza una petición PATCH a la API
        /// </summary>
        public async Task<OperationResult> PatchAsync<T>(string endpoint, int id, T data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Concatenar el ID directamente sin barra diagonal
                string url = $"{_baseUrl}/{endpoint}{id}";
                
                // Crear una solicitud PATCH
                var request = new HttpRequestMessage(new HttpMethod("PATCH"), url);
                request.Content = content;

                var response = await _httpClient.SendAsync(request);
                return await ProcessOperationResultAsync(response);
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al realizar PATCH: {ex.Message}");
            }
        }

        /// <summary>
        /// Procesa la respuesta HTTP y retorna el objeto deserializado
        /// </summary>
        private async Task<T> ProcessResponseAsync<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    var apiError = JsonConvert.DeserializeObject<ApiErrorResponse>(content);
                    if (apiError != null && !string.IsNullOrEmpty(apiError.Detail))
                    {
                        if (typeof(T) == typeof(OperationResult))
                        {
                            return (T)(object)OperationResult.Failure(apiError.Detail, apiError);
                        }

                        return default(T);
                    }
                }
                catch
                {
                    // Ignorar errores de deserialización y continuar
                }

                if (typeof(T) == typeof(OperationResult))
                {
                    return (T)(object)OperationResult.Failure($"Error: {response.ReasonPhrase}. Detalles: {content}");
                }

                return default(T);
            }

            try
            {
                var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);

                if (operationResult != null)
                {
                    if (operationResult.IsSuccess && operationResult.Data != null)
                    {
                        return JsonConvert.DeserializeObject<T>(
                            JsonConvert.SerializeObject(operationResult.Data));
                    }
                    else if (!operationResult.IsSuccess)
                    {
                        if (typeof(T) == typeof(OperationResult))
                        {
                            return (T)(object)operationResult;
                        }

                        return default(T);
                    }
                }
            }
            catch
            {
                // Ignorar errores de deserialización y continuar
            }

            try
            {
                var result = JsonConvert.DeserializeObject<T>(content);
                return result;
            }
            catch
            {
                if (typeof(T) == typeof(OperationResult))
                {
                    return (T)(object)OperationResult.Failure(
                        $"No se pudo deserializar la respuesta. Contenido: {content}");
                }

                return default(T);
            }
        }

        /// <summary>
        /// Procesa la respuesta HTTP para operaciones que devuelven OperationResult
        /// </summary>
        private async Task<OperationResult> ProcessOperationResultAsync(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    // Primero intentamos deserializar como ApiErrorResponse (formato detail/title/status)
                    var apiError = JsonConvert.DeserializeObject<ApiErrorResponse>(content);
                    if (apiError != null && !string.IsNullOrEmpty(apiError.Detail))
                    {
                        return OperationResult.Failure(apiError.Detail, apiError);
                    }
                }
                catch
                {
                    // Si no es un ApiErrorResponse, continuamos con el siguiente intento
                }

                try
                {
                    // Intentamos deserializar como OperationResult
                    var errorResult = JsonConvert.DeserializeObject<OperationResult>(content);
                    if (errorResult != null)
                    {
                        return errorResult;
                    }
                }
                catch
                {
                    // Si no es un OperationResult, continuamos
                }

                return OperationResult.Failure($"Error: {response.ReasonPhrase}. Detalles: {content}");
            }

            try
            {
                var result = JsonConvert.DeserializeObject<OperationResult>(content);
                return result ?? OperationResult.Success("Operación completada con éxito");
            }
            catch
            {
                return OperationResult.Success("Operación completada con éxito");
            }
        }
    }
}