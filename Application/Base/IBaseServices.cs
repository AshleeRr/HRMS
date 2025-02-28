using HRMS.Domain.Base;


namespace HRMS.Application.Base
{
    public interface IBaseServices<TAddDTO, TUpdateDTO, TRemoveDTO>
    {
        Task<OperationResult> GetAll();
        Task<OperationResult> GetById(int id);
        Task<OperationResult> Update(TUpdateDTO dto);

        Task<OperationResult> Save(TAddDTO dto);

        Task<OperationResult> Remove(TRemoveDTO dto);


    }
}
