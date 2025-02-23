using HRMS.Domain.Base;
using HRMS.Domain.Entities.Users;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IUsersRepository;
using HRMS.Persistence.Repositories.ValidationsRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace HRMS.Persistence.Repositories.ClientRepository
{
    public class ClientRepository : BaseRepository<Client, int>, IClientRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ClientRepository> _logger;
        public ClientRepository(HRMSContext context, ILogger<ClientRepository> logger,
                                                     IConfiguration configuration) : base(context)
        {
            _logger = logger;
            _configuration = configuration;
        }
        public async Task<IEnumerable<Client>> GetAllActiveClientsAsync() { 
            var clientes = await _context.Clients.Where(c => c.Estado == true).ToListAsync();
            if(!clientes.Any())
            {
                _logger.LogWarning("No se encontraron clientes activos");
            }
            return clientes;
        }
        public async Task<Client> VerifyEmailAsync(string correo) {
            if ( string.IsNullOrWhiteSpace(correo))
            {
                throw new ArgumentNullException(nameof(correo), "El correo no puede estar vacío");
            }
            var correoCliente = await _context.Clients.FirstOrDefaultAsync(c => c.Correo == correo);
            if (correoCliente == null)
            {
                _logger.LogWarning("No se encontró un cliente con ese correo");
            }
            return correoCliente;
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
                _logger.LogWarning("No se encontró un cliente con ese correo");
            }
            return cliente;
        }
        public async Task<List<Client>> GetClientsByDocumentAsync(string documento)
        {

            if (string.IsNullOrWhiteSpace(documento))
            {
                throw new ArgumentNullException(nameof(documento), "El correo no puede estar vacío");
            }
            var clientes = await _context.Clients.Where(c => c.Documento == documento).ToListAsync();
            if (!clientes.Any())
            {
                _logger.LogWarning("No se encontraron clientes con ese documento");
            }
            return clientes;
        }
        
        public override async Task<bool> ExistsAsync(Expression<Func<Client, bool>> filter)
        {
            if (filter == null)
            {
                return false;
            }
            return await base.ExistsAsync(filter);
        }
        public override async Task<OperationResult> GetAllAsync(Expression<Func<Client, bool>> filter)
        {
            OperationResult result = new OperationResult();
            try
            {
                if (filter != null)
                {
                    return await base.GetAllAsync(filter);
                }
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = _configuration["ErrorUserRepository: GetAllAsync"];
                result.IsSuccess = false;
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }
        public override async Task<Client> GetEntityByIdAsync(int id)
        {
            if(id <= 0)
            {
                throw new ArgumentNullException(nameof(id), "El id debe ser mayor que 0");
            }
            var entity = await _context.Clients.FindAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("No se encontró un cliente con ese id");
            }
            return entity;
        }

        public override async Task<OperationResult> SaveEntityAsync(Client entity) 
        {
            OperationResult resultSave = new OperationResult();
            try
            {
                if (!Validation.ValidateClient(entity, resultSave))
                    return resultSave;
                if(!Validation.ValidateId(entity.idCliente, resultSave))
                    return resultSave;
                if (!Validation.ValidateCorreo(entity.Correo, resultSave))
                    return resultSave;
                if (!Validation.ValidateTipoDocumento(entity.TipoDocumento, resultSave))
                    return resultSave;
                if (!Validation.ValidateDocumento(entity.Documento, resultSave))
                    return resultSave;
                if (!Validation.ValidateCompleteName(entity.NombreCompleto, resultSave))
                    return resultSave;
                if (!Validation.ValidateId((int)entity.IdUsuario, resultSave))
                    return resultSave;
                await _context.Clients.AddAsync(entity);
                await _context.SaveChangesAsync();
                resultSave.IsSuccess = true;
                _logger.LogInformation("Cliente guardado correctamente");
                return resultSave;
            }
            catch (Exception ex)
            {
                resultSave.Message = _configuration["ErrorClientRepository: SaveEntityAsync"];
                resultSave.IsSuccess = false;
                _logger.LogError(resultSave.Message, ex.ToString());
            }

            return resultSave;
        }
        public override async Task<OperationResult> UpdateEntityAsync(Client entity)
        {
            OperationResult resultUpdate = new OperationResult();
            try
            {
                if (!Validation.ValidateClient(entity, resultUpdate))
                    return resultUpdate;
                if (!Validation.ValidateCorreo(entity.Correo, resultUpdate))
                    return resultUpdate;
                if (!Validation.ValidateTipoDocumento(entity.TipoDocumento, resultUpdate))
                    return resultUpdate;
                if (!Validation.ValidateDocumento(entity.Documento, resultUpdate))
                    return resultUpdate;
                if (!Validation.ValidateCompleteName(entity.NombreCompleto, resultUpdate))
                    return resultUpdate;

                var ExistingClient = await _context.Clients.FindAsync(entity.idCliente);
                if (ExistingClient == null)
                {
                    resultUpdate.IsSuccess = false;
                    resultUpdate.Message = "Este cliente no existe";
                    return resultUpdate;
                }
                _context.Entry(ExistingClient).CurrentValues.SetValues(entity);
                
                await _context.SaveChangesAsync();
                resultUpdate.IsSuccess = true;
                resultUpdate.Message = "Cliente actualizado correctamente";
            }
            catch (Exception ex)
            {
                resultUpdate.Message = _configuration["ErrorClientRepository: UpdateEntityAsync"];
                resultUpdate.IsSuccess = false;
                _logger.LogError(resultUpdate.Message, ex.ToString());
            }
            return resultUpdate;
        }
    }
}
