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
            return await HandleResponseAsync<T>(response);
        }

        /// <summary>
        /// Realiza una petición GET a la API para obtener un elemento por ID
        /// </summary>
        public async Task<T> GetByIdAsync<T>(string endpoint, int id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/{endpoint}{id}");
            return await HandleResponseAsync<T>(response);
        }

        /// <summary>
        /// Realiza una petición POST a la API
        /// </summary>
        public async Task<OperationResult> PostAsync<T>(string endpoint, T data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/{endpoint}", content);
            return await HandleOperationResultAsync(response);
        }

        /// <summary>
        /// Realiza una petición PUT a la API
        /// </summary>
        public async Task<OperationResult> PutAsync<T>(string endpoint, int id, T data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_baseUrl}/{endpoint}{id}", content);
            return await HandleOperationResultAsync(response);
        }

        /// <summary>
        /// Realiza una petición DELETE a la API
        /// </summary>
        public async Task<OperationResult> DeleteAsync(string endpoint, int id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/{endpoint}{id}");
            return await HandleOperationResultAsync(response);
        }

        /// <summary>
        /// Procesa la respuesta y deserializa el contenido
        /// </summary>
        private async Task<T> HandleResponseAsync<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                    if (operationResult?.IsSuccess == true && operationResult.Data != null)
                    {
                        return JsonConvert.DeserializeObject<T>(
                            JsonConvert.SerializeObject(operationResult.Data));
                    }
                }
                catch
                {
                    // Si no es un OperationResult, intentamos deserializar directamente
                    return JsonConvert.DeserializeObject<T>(content);
                }
            }
            
            throw new ApiException(content, response.StatusCode);
        }

        /// <summary>
        /// Procesa la respuesta para operaciones que devuelven OperationResult
        /// </summary>
        private async Task<OperationResult> HandleOperationResultAsync(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return JsonConvert.DeserializeObject<OperationResult>(content) ?? 
                           OperationResult.Success();
                }
                catch
                {
                    return OperationResult.Success();
                }
            }
            
            try
            {
                return JsonConvert.DeserializeObject<OperationResult>(content) ?? 
                       OperationResult.Failure($"Error: {response.ReasonPhrase}");
            }
            catch
            {
                return OperationResult.Failure($"Error: {response.ReasonPhrase}");
            }
        }
    }
}