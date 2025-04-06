using System.Text;
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
            var response = await _httpClient.GetAsync($"{_baseUrl}/{endpoint}");
            return await ProcessResponseAsync<T>(response);
        }

        /// <summary>
        /// Realiza una petición GET a la API para obtener un elemento por ID
        /// </summary>
        public async Task<T> GetByIdAsync<T>(string endpoint, int id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/{endpoint}{id}");
            return await ProcessResponseAsync<T>(response);
        }

        /// <summary>
        /// Realiza una petición POST a la API
        /// </summary>
        public async Task<OperationResult> PostAsync<T>(string endpoint, T data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/{endpoint}", content);
            return await ProcessOperationResultAsync(response);
        }

        /// <summary>
        /// Realiza una petición PUT a la API
        /// </summary>
        public async Task<OperationResult> PutAsync<T>(string endpoint, int id, T data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_baseUrl}/{endpoint}{id}", content);
            return await ProcessOperationResultAsync(response);
        }

        /// <summary>
        /// Realiza una petición DELETE a la API
        /// </summary>
        public async Task<OperationResult> DeleteAsync(string endpoint, int id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/{endpoint}{id}");
            return await ProcessOperationResultAsync(response);
        }

        /// <summary>
        /// Procesa la respuesta HTTP y retorna el objeto deserializado
        /// </summary>
        private async Task<T> ProcessResponseAsync<T>(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new ApiException(errorContent, response.StatusCode);
            }

            var content = await response.Content.ReadAsStringAsync();
            
            try
            {
                // Primero intentamos deserializar como OperationResult
                var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                
                if (operationResult?.IsSuccess == true && operationResult.Data != null)
                {
                    // Si es un OperationResult exitoso, deserializamos su propiedad Data
                    return JsonConvert.DeserializeObject<T>(
                        JsonConvert.SerializeObject(operationResult.Data));
                }
                else if (operationResult?.IsSuccess == false)
                {
                    // Si es un OperationResult fallido, lanzamos una excepción
                    throw new ApiException(operationResult.Message ?? "Error en la operación", response.StatusCode);
                }
            }
            catch (JsonSerializationException)
            {
                // Si no es un OperationResult, continuamos con el siguiente intento
            }
            
            // Si no pudimos deserializar como OperationResult o si el objeto no era un OperationResult,
            // intentamos deserializar directamente al tipo solicitado
            try
            {
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (JsonSerializationException ex)
            {
                // Si tampoco podemos deserializar directamente, lanzamos una excepción detallada
                throw new ApiException(
                    $"No se pudo deserializar la respuesta. Contenido: {content}. Error: {ex.Message}",
                    response.StatusCode);
            }
        }

        /// <summary>
        /// Procesa la respuesta HTTP para operaciones que devuelven OperationResult
        /// </summary>
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
                        return new OperationResult
                        {
                            IsSuccess = false,
                            Message = apiError.Detail,
                            Data = apiError
                        };
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

                // Si no pudimos deserializar en ninguno de los formatos conocidos,
                // creamos un OperationResult genérico de error
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = $"Error: {response.ReasonPhrase}. Detalles: {content}"
                };
            }

            try
            {
                return JsonConvert.DeserializeObject<OperationResult>(content) ??
                       new OperationResult { IsSuccess = true, Message = "Operación completada con éxito" };
            }
            catch
            {
                // Si no podemos deserializar como OperationResult, creamos uno nuevo con éxito
                return new OperationResult { IsSuccess = true, Message = "Operación completada con éxito" };
            }
        }
    }
}