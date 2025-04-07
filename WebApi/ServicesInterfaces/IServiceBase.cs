using HRMS.WebApi.Models;

namespace WebApi.ServicesInterfaces
{
    public interface IServiceBase<TId,TDto, TDtoInsert, TDtoUpdate>
    {
        Task<OperationResult>  GetById(TId id);
        Task<OperationResult> GetAll();
        Task<OperationResult> Create(TDtoInsert dto);
        Task<OperationResult> Update(TDtoUpdate dto);
    }
}
