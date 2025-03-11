﻿using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.Users;
using HRMS.Domain.InfraestructureInterfaces.Logging;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IUsersRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace HRMS.Persistence.Repositories.UsersRepository
{
    public class UserRepository : BaseRepository<User, int>, IUserRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILoggingServices _loggerServices;
        private readonly ILogger<UserRepository> _logger;
        private readonly IValidator<User> _validator;

        public UserRepository(HRMSContext context, ILogger<UserRepository> logger,
                                                   ILoggingServices loggingServices,
                                                   IConfiguration configuration, IValidator<User> validator) : base(context)
        {
            _logger = logger;
            _configuration = configuration;
            _loggerServices = loggingServices;
            _validator = validator;
        }

        public async Task<List<User>> GetUsersByNameAsync(string nombreCompleto)
        {
            ArgumentException.ThrowIfNullOrEmpty(nombreCompleto, nameof(nombreCompleto));
            var usuarios = await _context.Users.Where(u => u.NombreCompleto == nombreCompleto).ToListAsync();
            if (!usuarios.Any())
            {
                _logger.LogWarning("No se encontraron usuarios activos");
            }
            return usuarios;
        }
        public async Task<User> GetUserByEmailAsync(string correo)
        {
            ArgumentException.ThrowIfNullOrEmpty(correo, nameof(correo));
            var usuario = await _context.Users.FirstOrDefaultAsync(u => u.Correo == correo);
            if (usuario == null)
            {
                _logger.LogWarning("No se encontró un usuario con ese correo");
            }
            return usuario;
        }
        public override async Task<OperationResult> GetAllAsync(Expression<Func<User, bool>> filter)
        {
            OperationResult result = new OperationResult();
            try
            {
                var usuarios = await _context.Users.Where(u => u.Estado == true).ToListAsync();
                if (!usuarios.Any())
                {
                    _logger.LogWarning("No se encontraron usuarios activos");
                }
                result.Data = usuarios;
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result = await _loggerServices.LogError(ex.Message, this);
            }
            return result;
        }
        public override async Task<User> GetEntityByIdAsync(int id)
        {
            var entityById = await _context.Users.FindAsync(id);
            if (entityById == null)
            {
                _logger.LogWarning("No se encontró un usuario con ese id");
            }
            return entityById;
        }
        public override async Task<OperationResult> SaveEntityAsync(User entity)
        {
            OperationResult result = new OperationResult();
            try
            {
                var validUser = _validUser(entity);
                if(!validUser.IsSuccess)
                {
                    return validUser;
                }
                entity.FechaCreacion = DateTime.Now;
                result.IsSuccess = true;
                await _context.Users.AddAsync(entity);
                await _context.SaveChangesAsync();
                result.Message = "Usuario guardado correctamente";
            }
            catch (Exception ex)
            {
                result = await _loggerServices.LogError(ex.Message, this);
            }
            return result;
        }
        private OperationResult _validUser(User user)
        {
            return _validator.Validate(user);
        }
        public override async Task<OperationResult> UpdateEntityAsync(User entity)
        {
            OperationResult result = new OperationResult();
            try
            {
                var validUser = _validUser(entity);
                if (!validUser.IsSuccess)
                {
                    return validUser;
                }
                var userExistente = await _context.Users.FindAsync(entity.IdUsuario);
                if (userExistente == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Este usuario no existe";
                    return result;
                }
                var usuario = await _context.Users.FindAsync(entity.IdUsuario);
                usuario.Clave = entity.Clave;
                usuario.NombreCompleto = entity.NombreCompleto;
                usuario.Correo = entity.Correo;

                _context.Users.Update(usuario);
                await _context.SaveChangesAsync();
                result.IsSuccess = true;    
                result.Message = "Usuario actualizado correctamente";
            }
            catch (Exception ex)
            {
                result = await _loggerServices.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<User> GetUserByDocumentAsync(string documento)
        {

            if (string.IsNullOrWhiteSpace(documento))
            {
                throw new ArgumentNullException(nameof(documento), "El documento no puede estar vacío");
            }
            var usuario = await _context.Users.FirstOrDefaultAsync(u => u.Documento == documento);

            if (usuario == null)
            {
                _logger.LogWarning("No se encontró un cliente con ese correo");
            }
            return usuario;
        }

        public async Task<List<User>> GetUsersByTypeDocumentAsync(string tipoDocumento)
        {
            if (string.IsNullOrWhiteSpace(tipoDocumento))
            {
                throw new ArgumentNullException(nameof(tipoDocumento), "El tipo de documento no puede estar vacío");
            }
            var usuarios = await _context.Users.Where(u => u.TipoDocumento == tipoDocumento).ToListAsync();
            if (!usuarios.Any())
            {
                _logger.LogWarning("No se encontraron clientes con ese tipo de documento");
            }
            return usuarios;
        }
        
    }
}
