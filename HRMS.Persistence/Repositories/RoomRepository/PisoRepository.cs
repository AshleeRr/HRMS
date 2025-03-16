using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HRMS.Persistence.Repositories.RoomRepository
{
    public class PisoRepository : BaseRepository<Piso, int>, IPisoRepository
    {
        private readonly ILogger<PisoRepository> _logger;
        private readonly IValidator<Piso> _validator;
        public PisoRepository(HRMSContext context, ILogger<PisoRepository> logger, IValidator<Piso> validator) : base(context)
        {
            _logger = logger;
            _validator = validator;
        }

        public override async Task<List<Piso>> GetAllAsync()
        {
            _logger.LogInformation("Obteniendo todos los pisos activos");
            return await _context.Pisos.Where(p => p.Estado == true).ToListAsync();
        }
        
        public override async Task<OperationResult> SaveEntityAsync(Piso piso)
        {
            try
            {
                _logger.LogInformation("Guardando nuevo piso");
                var validationResult = _validator.Validate(piso);
                if (!validationResult.IsSuccess)
                {
                    _logger.LogWarning("Error de validación al guardar piso: {Error}", validationResult.Message);
                    return OperationResult.Failure(validationResult.Message);
                }
                
                await _context.Pisos.AddAsync(piso);
                await _context.SaveChangesAsync();
                return OperationResult.Success(piso, "Piso guardada exitosamente.");
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al guardar habitación: {ex.Message}");
            }
        }

        public async override Task<OperationResult> UpdateEntityAsync(Piso entity)
        {
            try
            {
                _logger.LogInformation("Actualizando piso");
                var validationResult = _validator.Validate(entity);
                if (!validationResult.IsSuccess)
                {
                    _logger.LogWarning("Error de validación al actualizar piso: {Error}", validationResult.Message);
                    return OperationResult.Failure(validationResult.Message);
                }
                var existingPiso = await _context.Pisos.FindAsync(entity.IdPiso);
                if (existingPiso == null)
                    return OperationResult.Failure("El piso no existe.");

                existingPiso.Descripcion = entity.Descripcion;

                await _context.SaveChangesAsync();

                return OperationResult.Success(existingPiso, "Habitación actualizada correctamente.");
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al actualizar piso: {ex.Message}");
            }
        }

        public override async Task<Piso> GetEntityByIdAsync(int id)
        {
            _logger.LogInformation($"Obteniendo piso por ID: {id}");
            
            return (id == 0 ? null : await _context.Pisos
                .FirstOrDefaultAsync(p => p.IdPiso == id && p.Estado == true))!;
        }

        public async Task<OperationResult> GetPisoByDescripcion(string descripcion)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                _logger.LogInformation($"Buscando pisos por descripción '{descripcion}'");
                
                var validationResult = ValidateString(descripcion, "La descripción no puede estar vacía.");
                if (!validationResult.IsSuccess)
                {
                    return validationResult;
                }
                var pisos = await _context.Pisos
                    .Where(p => p.Descripcion != null && 
                                EF.Functions.Like(p.Descripcion, $"%{descripcion}%") && 
                                p.Estado == true)
                    .ToListAsync();
            
                return pisos.Any() 
                    ? OperationResult.Success(pisos) 
                    : OperationResult.Failure($"No se encontraron pisos con la descripción '{descripcion}'.");
            });
        }

        
        public virtual async Task<bool> ExistsByDescripcionAsync(string descripcion, int excludePisoId = 0)
        {
            return await _context.Pisos.AnyAsync(p => 
                p.Descripcion == descripcion && 
                p.IdPiso != excludePisoId && 
                p.Estado == true);
        }
        
        private static OperationResult ValidateString( string value, string message)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return OperationResult.Failure(message);
            }
            return OperationResult.Success();
        }
    }
}