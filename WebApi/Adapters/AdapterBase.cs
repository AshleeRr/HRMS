using HRMS.WebApi.Logging;
using HRMS.WebApi.Models;

namespace WebApi.Adapters
{
    public abstract class AdapterBase<TId, TDto, TDtoInsert, TDtoUpdate>
    {
        private readonly HttpClient _httpClient;
        private readonly string _controller;
        private readonly ILoggingServices _loggingServices;


        public AdapterBase(IHttpClientFactory httpClientFactory,ILoggingServices loggingServices, string controller)
        {
            _loggingServices = loggingServices;
            _controller = controller;
            _httpClient = httpClientFactory.CreateClient(controller);
            //s
        }

        public async Task<OperationResult> GetAll(string endpoint = "GetAll")
        {
            OperationResult result = new();
            try
            {
                var response = await _httpClient.GetAsync($"{endpoint}");
                if (response.IsSuccessStatusCode)
                {
                    result.Data = await response.Content.ReadFromJsonAsync<List<TDto>>();
                }
                else
                {
                    string errors = await response.Content.ReadAsStringAsync();
                    result.Message = "Error al cargar las reservas" + errors;
                    result.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                result = await _loggingServices.LogError(ex.Message, this);

            }
            return result;

        }

        public async Task<OperationResult> GetById( TId id, string endpoint = "GetByID")
        {
            OperationResult result = new();
            try
            {
                var response = await _httpClient.GetAsync($"{endpoint}/{ id}");
                if (response.IsSuccessStatusCode)
                {
                    result.Data = await response.Content.ReadFromJsonAsync<TDto>();
                }
                else
                {
                    string errors = await response.Content.ReadAsStringAsync();
                    result.Message = "Error al cargar la reserva" + errors;
                    result.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                result = await _loggingServices.LogError(ex.Message, this);

            }
            return result;



        }

        public async Task<OperationResult> Post(TDtoInsert dto, string endpoint = "Create")
        {
            OperationResult result = new();
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{endpoint}", dto);
                if (!response.IsSuccessStatusCode)
                {
                    string errors = await response.Content.ReadAsStringAsync();
                    result.Message = "Error al crear la reserva" + errors;
                    result.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                result = await _loggingServices.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<OperationResult> Put(TDtoUpdate dto,string endpoint = "Update")
        {
            OperationResult result = new();
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{endpoint}", dto);
                if (!response.IsSuccessStatusCode)
                {
                    string errors = await response.Content.ReadAsStringAsync();
                    result.Message = "Error al actualizar la reserva" + errors;
                    result.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                result = await _loggingServices.LogError(ex.Message, this);
            }
            return result;
        }
    }
}
