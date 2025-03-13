using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.Users;
using HRMS.Domain.InfraestructureInterfaces.Logging;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IUsersRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;

namespace HRMS.Persistence.Repositories.UsersRepository
{
    public class ClientRepository : BaseRepository<Client, int>, IClientRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILoggingServices _loggerServices;
        private readonly IValidator<Client> _validator;
        public ClientRepository(HRMSContext context, ILoggingServices loggingServices,
                                                     IConfiguration configuration, IValidator<Client> validator) : base(context)
        {
            _configuration = configuration;
            _loggerServices = loggingServices;
            _validator = validator;
        }
       
        public async Task<Client> GetClientByEmailAsync(string correo) 
        {
            if (string.IsNullOrWhiteSpace(correo))
            {
                throw new ArgumentNullException(nameof(correo), "El correo no puede estar vacío");
            }
            var cliente = await _context.Clients.FirstOrDefaultAsync(c => c.Correo == correo);
            if (cliente == null) 
            {
                await _loggerServices.LogWarning("No se encontró un cliente con este correo", this, nameof(GetClientByEmailAsync));
            }
            return cliente;
        }
        public async Task<Client> GetClientByDocumentAsync(string documento)
        {

            if (string.IsNullOrWhiteSpace(documento))
            {
                throw new ArgumentNullException(nameof(documento), "El documento no puede estar vacío");
            }
            var cliente = await _context.Clients.FirstOrDefaultAsync(c => c.Documento == documento);
            if (cliente == null)
            {
                await _loggerServices.LogWarning("No se encontró un cliente con este documento", this, nameof(GetClientByDocumentAsync));
            }
            return cliente;
        }
        public async Task<List<Client>> GetClientsByTypeDocumentAsync(string tipoDocumento)
        {

            if (string.IsNullOrWhiteSpace(tipoDocumento))
            {
                throw new ArgumentNullException(nameof(tipoDocumento), "El tipo de documento no puede estar vacío");
            }
            var clientes = await _context.Clients.Where(c => c.TipoDocumento == tipoDocumento).ToListAsync();
            if (!clientes.Any())
            {
                await _loggerServices.LogWarning("No se encontraron clientes con ese tipo de documento", this, nameof(GetClientsByTypeDocumentAsync));
            }
            return clientes;
        }
        public async Task<Client> GetClientByUserIdAsync(int idUsuario)
        {
            return await _context.Clients.FirstOrDefaultAsync(c => c.IdUsuario == idUsuario);
        }
        public override async Task<OperationResult> GetAllAsync(Expression<Func<Client, bool>> filter)
        {
            OperationResult result = new OperationResult();
            try
            {
                var clientes = await _context.Clients.Where(c => c.Estado == true).ToListAsync();
                if (!clientes.Any())
                {
                    await _loggerServices.LogWarning("No se encontraron clientes activos", this, nameof(GetAllAsync));
                }
                result.Data = clientes; 
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result = await _loggerServices.LogError(ex.Message, this);
            }
            return result;
        }
        
        public override async Task<Client> GetEntityByIdAsync(int id)
        {
            if(id < 1)
            {
                throw new ArgumentNullException(nameof(id), "El id debe ser mayor que 0");
            }
            var entity = await _context.Clients.FindAsync(id);
            if (entity == null)
            {
                await _loggerServices.LogWarning("No se encontró un cliente con ese id", this, nameof(GetEntityByIdAsync));
            }
            return entity;
        }
        private OperationResult _validClient(Client client)
        {
            return _validator.Validate(client);
        }

        public override async Task<OperationResult> UpdateEntityAsync(Client entity)
        {
            OperationResult result = new OperationResult();
            try
            {
                var validClient = _validClient(entity);
                if (!validClient.IsSuccess)
                {
                    return validClient;
                }
                var cliente = await _context.Clients.FindAsync(entity.IdUsuario);
                if (cliente == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Este cliente no existe";
                    return result;
                }
                cliente.TipoDocumento = entity.TipoDocumento;
                cliente.Documento = entity.Documento;
                cliente.Correo = entity.Correo;
                cliente.NombreCompleto = entity.NombreCompleto;
                cliente.Clave = entity.Clave;

                _context.Clients.Update(cliente);
                await _context.SaveChangesAsync();
                result.IsSuccess = true;
                result.Message = "Cliente actualizado correctamente";
            }
            catch (Exception ex)
            {
                result = await _loggerServices.LogError(ex.Message, this);
            }
            return result;
        }
        public override async Task<OperationResult> SaveEntityAsync(Client entity)
        {
            OperationResult result = new OperationResult();
            try
            {
                var validClient = _validClient(entity);
                if (!validClient.IsSuccess)
                {
                    return validClient;
                }
                entity.Estado = true;
                entity.FechaCreacion = DateTime.Now;
                result.IsSuccess = true;
                await _context.Clients.AddAsync(entity);
                await _context.SaveChangesAsync();
                result.Message = "Cliente guardado correctamente";
                result.Data = entity;
            }
            catch (Exception ex)
            {
                result = await _loggerServices.LogError(ex.Message, this);
            }
            return result;
        }
    }
}
