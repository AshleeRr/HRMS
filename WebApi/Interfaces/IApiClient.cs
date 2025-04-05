using WebApi.Models;

namespace WebApi.Interfaces
{
    public interface IApiClient
    {
        Task<T> GetAsync<T>(string endpoint);
        Task<T> GetByIdAsync<T>(string endpoint, int id);
        Task<OperationResult> PostAsync<T>(string endpoint, T data);
        Task<OperationResult> PutAsync<T>(string endpoint, int id, T data);
        Task<OperationResult> DeleteAsync(string endpoint, int id);
    }
}