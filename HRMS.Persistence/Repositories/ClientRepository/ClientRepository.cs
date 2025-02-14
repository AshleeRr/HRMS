using HRMS.Domain.Base;
using HRMS.Domain.Entities.Reservation;
using HRMS.Domain.Entities.Users;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IUsersRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace HRMS.Persistence.Repositories.ClientRepository
{
    public class ClientRepository : BaseRepository<Client, int>, IClientRepository
    {
        private readonly HRMSContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ClientRepository> _logger;
        public ClientRepository(HRMSContext context, ILogger<ClientRepository> logger,
                                                     IConfiguration configuration) : base(context)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<Client> GetClientByCorreo(string correo)
        {
            ArgumentException.ThrowIfNullOrEmpty(correo, nameof(correo));
            var cliente = await _context.Clients.FirstOrDefaultAsync(c => c.Correo == correo);
            if (cliente == null)
            {
                _logger.LogWarning("No se encontró un cliente con ese correo");
            }
            return cliente;
        }
        public async Task<List<Client>> GetClientsByDocument(string documento)
        {
            ArgumentException.ThrowIfNullOrEmpty(documento, nameof(documento));
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
                if (!ValidateUser(entity, resultSave))
                    return resultSave;

                _context.Clients.Add(entity);
                await _context.SaveChangesAsync();
                resultSave.IsSuccess = true;

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
                if (!ValidateUser(entity, resultUpdate))
                    return resultUpdate;

                _context.Clients.Update(entity);
                await _context.SaveChangesAsync();
                resultUpdate.IsSuccess = true;
            }
            catch (Exception ex)
            {
                resultUpdate.Message = _configuration["ErrorClientRepository: UpdateEntityAsync"];
                resultUpdate.IsSuccess = false;
                _logger.LogError(resultUpdate.Message, ex.ToString());
            }
            return resultUpdate;
        }
        private bool ValidateUser(Client entity, OperationResult result)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "El cliente no puede ser nulo.");
            }
            return true;
        }
    }
}
