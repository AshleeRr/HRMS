using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HRMS.Persistence.Repositories.RoomRepository
{
    public class PisoRepository : BaseRepository<Piso, int>, IPisoRepository
    {
        private readonly ILogger<PisoRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly IValidator<Piso> _validator;

        public PisoRepository(HRMSContext context, ILogger<PisoRepository> logger,
            IConfiguration configuration, IValidator<Piso> validator) : base(context)
        {
            _logger = logger;
            _configuration = configuration;
            _validator = validator;
        }

        public override async Task<List<Piso>> GetAllAsync()
        {
            return await _context.Pisos.Where(p => p.Estado == true).ToListAsync();
        }

        public override async Task<Piso?> GetEntityByIdAsync(int id)
        {
            return id != 0 ? await _context.Set<Piso>().FindAsync(id) : null;
        }

        public async Task<OperationResult> GetPisoByDescripcion(string descripcion)
        {
            return await GetByFilterAsync(
                "La descripción del piso no puede estar vacía.",
                descripcion,
                _context.Pisos.Where(p => p.Descripcion != null && EF.Functions.Like(p.Descripcion , 
                $"%{descripcion}%" ) && p.Estado == true),
                $"No se encontraron pisos con la descripción '{descripcion}'."
            );
        }
          

        public override async Task<OperationResult> UpdateEntityAsync(Piso piso)
        {
            var result = ValidatePiso(piso);
            if (!result.IsSuccess) return result;

            try
            {
                var existingPiso = await _context.Pisos.FindAsync(piso.IdPiso);
                if (existingPiso == null)
                {
                    return new OperationResult
                    {
                        IsSuccess = false,
                        Message = "El piso no existe."
                    };
                }

                if (await _context.Pisos.AnyAsync(p => p.Descripcion == piso.Descripcion && p.IdPiso != piso.IdPiso))
                {
                    return new OperationResult
                    {
                        IsSuccess = false,
                        Message = $"Ya existe otro piso con la descripción '{piso.Descripcion}'."
                    };
                }

                UpdatePiso(existingPiso, piso);
                await _context.SaveChangesAsync();

                return new OperationResult
                {
                    IsSuccess = true,
                    Message = "Piso actualizado correctamente.",
                    Data = existingPiso
                };
            }
            catch (DbUpdateConcurrencyException)
            {
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = "El piso fue modificado por otro usuario. Intente nuevamente."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando el piso.");
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = $"Ocurrió un error actualizando el piso: {ex.Message}"
                };
            }
        }


        private OperationResult ValidatePiso(Piso piso)
        {
            var validation = _validator.Validate(piso);
            return validation.IsSuccess ? new OperationResult { IsSuccess = true } : validation;
        }

        private void UpdatePiso(Piso existing, Piso updated)
        {
            existing.Descripcion = updated.Descripcion;
            existing.Estado = updated.Estado;
        }

        private async Task<OperationResult> GetByFilterAsync<T>(
            string? validationMessage, string? filterValue,
            IQueryable<T> query, string notFoundMessage)
        {
            if (!string.IsNullOrWhiteSpace(validationMessage) && string.IsNullOrWhiteSpace(filterValue))
            {
                return new OperationResult { IsSuccess = false, Message = validationMessage };
            }

            try
            {
                var resultList = await query.ToListAsync();
                return new OperationResult
                {
                    IsSuccess = resultList.Any(),
                    Message = resultList.Any() ? null : notFoundMessage,
                    Data = resultList
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo datos.");
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = $"Ocurrió un error obteniendo datos: {ex.Message}"
                };
            }
        }
    }
}
