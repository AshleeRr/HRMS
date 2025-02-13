using HRMS.Domain.Base;
using HRMS.Domain.Entities.Users;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces;

namespace HRMS.Persistence.Repositories
{
    public class ClientRepository : BaseRepository<Client, int>, IClientRepository
    {
        //tomar en cuenta que no puede llegar null a baseRepository 
        
        public ClientRepository(HRMSContext context) : base(context) 
        { 
        }

        public OperationResult GetClientByClientId(int IdCliente)
        {
            throw new NotImplementedException();
        }

        public Task<Client> GetClientByCorreo(string correo)
        {
            throw new NotImplementedException();
        }

        public Task<List<Client>> GetClientsByDocument(string documento)
        {
            throw new NotImplementedException();
        }

        //sobreescribir todos los metodos
        public override Task<OperationResult> SaveEntityAsync(Client entity)
        {
            //agregar las validaiciones correspondientes
            return base.SaveEntityAsync(entity);
        }

        Task<Client> IClientRepository.GetClientByClientId(int IdCliente)
        {
            throw new NotImplementedException();
        }
    }
}
