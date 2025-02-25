﻿using HRMS.Domain.Base;
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
        public async Task<Client> GetClientByDocumentAsync(string documento)
        {

            if (string.IsNullOrWhiteSpace(documento))
            {
                throw new ArgumentNullException(nameof(documento), "El documento no puede estar vacío");
            }
            var cliente = await _context.Clients.FirstOrDefaultAsync(c => c.Documento == documento);
            if (cliente == null)
            {
                _logger.LogWarning("No se encontró un cliente con este documento");
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
                _logger.LogWarning("No se encontraron clientes con ese tipo de documento");
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
                var clientes = await _context.Clients.Where(c => c.Estado == true).ToListAsync();
                if (!clientes.Any())
                {
                    _logger.LogWarning("No se encontraron clientes activos");
                }
                result.Data = clientes; 
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
            if(id < 1)
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
                {
                    _logger.LogWarning("Falló ValidateClient: " + resultSave.Message);
                    return resultSave;
                }
                if (!await Validation.ValidateCorreo(entity.Correo, entity.IdCliente, _context, resultSave))
                {
                    _logger.LogWarning("Falló ValidateCorreo: " + resultSave.Message);
                    return resultSave;
                }
                if (!Validation.ValidateTipoDocumento(entity.TipoDocumento, entity.IdCliente, resultSave))
                {
                    _logger.LogWarning("Falló ValidateTipoDocumento: " + resultSave.Message);
                    return resultSave;
                }
                if (!await Validation.ValidateDocumento(entity.Documento, entity.IdCliente, _context, resultSave))
                {
                    _logger.LogWarning("Falló ValidateDocumento: " + resultSave.Message);
                    return resultSave;
                }
                if (!Validation.ValidateCompleteName(entity.NombreCompleto, entity.IdCliente, resultSave))
                {
                    _logger.LogWarning("Falló ValidateCompleteName: " + resultSave.Message);
                    return resultSave;
                }

                resultSave.IsSuccess = true;
                await _context.Clients.AddAsync(entity);
                await _context.SaveChangesAsync();

                resultSave.Message = "Cliente guardado existosamente";
                return resultSave;
            }
            catch (Exception ex)
            {
                resultSave.Message = _configuration["ErrorClientRepository: SaveEntityAsync"];
                resultSave.IsSuccess = false;

                _logger.LogError($"Error en SaveEntityAsync: {ex.Message}");
                if (ex.InnerException != null)
                {
                    _logger.LogError($"InnerException: {ex.InnerException.Message}");
                }
                _logger.LogError($"StackTrace: {ex.StackTrace}");
                Console.WriteLine("mensaje que entro al catch");
            }

            return resultSave;

        }
        public override async Task<OperationResult> UpdateEntityAsync(Client entity)
        {
            OperationResult resultUpdate = new OperationResult();
            try
            {
                var existingClient = await _context.Clients.FindAsync(entity.IdCliente);
                if(existingClient == null)
                {
                    resultUpdate.IsSuccess = false;
                    resultUpdate.Message = "Cliente no encontrado";
                    return resultUpdate; ;
                }
                
                if (!Validation.ValidateClient(entity, resultUpdate))
                    return resultUpdate;
                if (!await Validation.ValidateCorreo(entity.Correo, entity.IdCliente, _context, resultUpdate))
                    return resultUpdate;
                if (!Validation.ValidateTipoDocumento(entity.TipoDocumento, entity.IdCliente, resultUpdate))
                    return resultUpdate;
                if (!await Validation.ValidateDocumento(entity.Documento, entity.IdCliente, _context, resultUpdate))
                    return resultUpdate;
                if (!Validation.ValidateCompleteName(entity.NombreCompleto, entity.IdCliente, resultUpdate))
                    return resultUpdate;
                _context.Clients.Update(entity);
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
