using HRMS.Domain.Entities.Users;
using HRMS.Domain.Repository;
using HRMS.Domain.Base;

namespace HRMS.Persistence.Interfaces
{
    public interface IClientRepository : IBaseRepository<Client, int>
    {
        //Poner los metodos que son exclusivos de la entidad
        OperationResult GetClientByClientId(int IdCliente);
    }
}
