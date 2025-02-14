using HRMS.Domain.Base;
using HRMS.Domain.Entities.Users;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HRMS.Persistence.Repositories
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

        public async Task<Client> GetClientByClientId(int idCliente)
        {
            if (idCliente < 1)
            {
                throw new ArgumentException("El ID del cliente no puede ser menor a 1", nameof(idCliente));
            }
            var cliente = await _context.Clients.FindAsync(idCliente);
            if (cliente == null)
            {
                _logger.LogWarning("No se encontró un cliente con ese id");
            }
            return cliente;
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
    }
}
