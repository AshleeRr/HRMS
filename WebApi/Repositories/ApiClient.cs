using System.Text;
using Newtonsoft.Json;
using WebApi.Interfaces;
using WebApi.Models;

namespace WebApi.Repositories
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ApiClient(string baseUrl)
        {
            _httpClient = new HttpClient();
            _baseUrl = baseUrl.TrimEnd('/');
        }

        public Task<T> GetAsync<T>(string endpoint) =>
            SendRequestAsync<T>(HttpMethod.Get, BuildUrl(endpoint));

        public Task<T> GetByIdAsync<T>(string endpoint, int id) =>
            SendRequestAsync<T>(HttpMethod.Get, BuildUrl(endpoint, id));

        public Task<OperationResult> PostAsync<T>(string endpoint, T data) =>
            SendDataAsync(HttpMethod.Post, BuildUrl(endpoint), data);

        public Task<OperationResult> PutAsync<T>(string endpoint, int id, T data) =>
            SendDataAsync(HttpMethod.Put, BuildUrl(endpoint, id), data);

        public async Task<OperationResult> DeleteAsync(string endpoint, int id)
        {
            var response = await _httpClient.DeleteAsync(BuildUrl(endpoint, id));
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode && content.Contains("habitaciones asociadas"))
            {
                return OperationResult.Failure("No se puede eliminar porque tiene elementos asociados. Debe eliminar o reubicar estos elementos primero.");
            }

            return await ProcessOperationResultAsync(response);
        }

        public Task<OperationResult> PatchAsync<T>(string endpoint, int id, T data)
        {
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), BuildUrl(endpoint, id))
            {
                Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
            };

            return SendRequestWithOperationResultAsync(request);
        }

        private string BuildUrl(string endpoint, int? id = null)
        {
            var cleanEndpoint = endpoint.Trim('/');
            return id.HasValue
                ? $"{_baseUrl}/{cleanEndpoint}{id.Value}"
                : $"{_baseUrl}/{cleanEndpoint}";
        }

        private async Task<T> SendRequestAsync<T>(HttpMethod method, string url)
        {
            try
            {
                var response = await _httpClient.SendAsync(new HttpRequestMessage(method, url));
                var result = await ProcessResponseAsync<T>(response);

                if (EqualityComparer<T>.Default.Equals(result, default))
                {
                    if (typeof(T) == typeof(OperationResult))
                    {
                        return (T)(object)OperationResult.Failure($"Error al obtener datos desde {url}");
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

                return default;
            }
        }

        private Task<OperationResult> SendDataAsync<T>(HttpMethod method, string url, T data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(method, url) { Content = content };

            return SendRequestWithOperationResultAsync(request);
        }

        private async Task<OperationResult> SendRequestWithOperationResultAsync(HttpRequestMessage request)
        {
            try
            {
                var response = await _httpClient.SendAsync(request);
                return await ProcessOperationResultAsync(response);
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error en la solicitud HTTP: {ex.Message}");
            }
        }

        private async Task<T> ProcessResponseAsync<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return HandleApiError<T>(content, response.ReasonPhrase!);
            }

            try
            {
                var operationResult = JsonConvert.DeserializeObject<OperationResult>(content);
                if (operationResult?.IsSuccess == true && operationResult.Data != null)
                {
                    return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(operationResult.Data))!;
                }

                if (typeof(T) == typeof(OperationResult))
                {
                    return (T)(object)(operationResult ?? OperationResult.Failure("Respuesta inválida de la API"));
                }
            }
            catch { }

            try
            {
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch
            {
                if (typeof(T) == typeof(OperationResult))
                {
                    return (T)(object)OperationResult.Failure($"No se pudo deserializar la respuesta: {content}");
                }

                return default;
            }
        }

        private T HandleApiError<T>(string content, string reason)
        {
            try
            {
                var apiError = JsonConvert.DeserializeObject<ApiErrorResponse>(content);
                if (!string.IsNullOrEmpty(apiError?.Detail) && typeof(T) == typeof(OperationResult))
                {
                    return (T)(object)OperationResult.Failure(apiError.Detail, apiError);
                }
            }
            catch { }

            if (typeof(T) == typeof(OperationResult))
            {
                return (T)(object)OperationResult.Failure($"Error: {reason}. Detalles: {content}");
            }

            return default;
        }

        private async Task<OperationResult> ProcessOperationResultAsync(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return HandleOperationError(content, response.ReasonPhrase);
            }

            try
            {
                return JsonConvert.DeserializeObject<OperationResult>(content) ?? OperationResult.Success("Operación completada con éxito");
            }
            catch
            {
                return OperationResult.Success("Operación completada con éxito");
            }
        }

        private OperationResult HandleOperationError(string content, string reason)
        {
            try
            {
                var apiError = JsonConvert.DeserializeObject<ApiErrorResponse>(content);
                if (!string.IsNullOrEmpty(apiError?.Detail))
                {
                    return OperationResult.Failure(apiError.Detail, apiError);
                }
            }
            catch { }

            try
            {
                var opResult = JsonConvert.DeserializeObject<OperationResult>(content);
                if (opResult != null)
                    return opResult;
            }
            catch { }

            return OperationResult.Failure($"Error: {reason}. Detalles: {content}");
        }
    }
}
