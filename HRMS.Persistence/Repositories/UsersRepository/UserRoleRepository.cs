﻿using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.Users;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IUsersRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace HRMS.Persistence.Repositories.UsersRepository
{
    public class UserRoleRepository : BaseRepository<UserRole, int>, IUserRoleRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserRoleRepository> _logger;
        private readonly IValidator<UserRole> _validator;
        public UserRoleRepository(HRMSContext context, ILogger<UserRoleRepository> logger,
                                                     IConfiguration configuration, IValidator<UserRole> validator) : base(context)
        {
            _logger = logger;
            _configuration = configuration;
            _validator = validator;
        }

        public async Task<UserRole> GetRoleByDescriptionAsync (string descripcion)
        {
            return await _context.UserRoles.AsNoTracking().FirstOrDefaultAsync(ur => ur.Descripcion == descripcion);
         
        }
        public override async Task<bool> ExistsAsync(Expression<Func<UserRole, bool>> filter)
        {
            if (filter == null)
            {
                return false;
            }
            return await base.ExistsAsync(filter);
        }
        public override async Task<OperationResult> GetAllAsync(Expression<Func<UserRole, bool>> filter)
        {
            OperationResult result = new OperationResult();
            try
            {
                var roles = await _context.UserRoles.Where(ur => ur.Estado == true).ToListAsync();
                if (!roles.Any())
                {
                    _logger.LogWarning("No se encontraron roles activos");
                }
                result.Data = roles;
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = _configuration["ErrorUserRolRepository: GetAllAsync"];
                result.IsSuccess = false;
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        public override async Task<UserRole> GetEntityByIdAsync(int id)
        {
            var entity = await _context.UserRoles.FindAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("No se encontró un cliente con ese id");
            }
            return entity;
        }
        public override async Task<OperationResult> SaveEntityAsync(UserRole entity)
        {
            OperationResult resultSave = new OperationResult();
            try
            {
                var validUserRole = _validator.Validate(entity);
                if (!validUserRole.IsSuccess)
                {
                    return validUserRole;
                }
                entity.FechaCreacion = DateTime.Now;
                resultSave.IsSuccess = true;
                await _context.UserRoles.AddAsync(entity);
                await _context.SaveChangesAsync();

                resultSave.Message = "Rol de usuario guardado correctamente.";
                return resultSave;
            }
            catch (Exception ex)
            {
                resultSave.Message = _configuration["ErrorUserRolRepository: SaveEntityAsync"];
                resultSave.IsSuccess = false;
                _logger.LogError(resultSave.Message, ex.ToString());
            }
            return resultSave;
        }
        private OperationResult _validUserRoleForUpdateMethod(UserRole userRole)
        {
            return _validator.Validate(userRole);
        }
        public override async Task<OperationResult> UpdateEntityAsync(UserRole entity)
        {
            OperationResult result = new OperationResult();
            try
            {
                var validUserRole = _validUserRoleForUpdateMethod(entity);
                if(!validUserRole.IsSuccess)
                {
                    return validUserRole;
                }
                var rolUsuario = await _context.UserRoles.FindAsync(entity.IdRolUsuario);
                if (rolUsuario == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Este rol no existe";
                    return result;
                }

                rolUsuario.Descripcion = entity.Descripcion;
                rolUsuario.RolNombre = entity.RolNombre;

                _context.UserRoles.Update(rolUsuario);
                await _context.SaveChangesAsync();
                result.IsSuccess = true;
                result.Message = "Rol de usuario actualizado correctamente.";
            }
            catch (Exception ex)
            {
                result.Message = _configuration["ErrorUserRolRepository: UpdateEntityAsync"];
                result.IsSuccess = false;
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        public async Task<UserRole> GetRoleByNameAsync(string rolNombre)
        {
            return await _context.UserRoles.AsNoTracking().FirstOrDefaultAsync(ur => ur.RolNombre == rolNombre);
        }
    }
}
