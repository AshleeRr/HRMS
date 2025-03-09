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
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(descripcion))
                    return OperationResult.Failure("La descripción del piso no puede estar vacía.");
            
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

        public override async Task<OperationResult> UpdateEntityAsync(Piso piso)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                var result = ValidatePiso(piso);
                if (!result.IsSuccess) return result;

                var existingPiso = await _context.Pisos.FindAsync(piso.IdPiso);
                if (existingPiso == null)
                {
                    return OperationResult.Failure("El piso no existe.");
                }

                if (await _context.Pisos.AnyAsync(p => p.Descripcion == piso.Descripcion && p.IdPiso != piso.IdPiso))
                {
                    return OperationResult.Failure($"Ya existe un piso con la descripción '{piso.Descripcion}'.");
                }

                UpdatePiso(existingPiso, piso);
                await _context.SaveChangesAsync();

                return OperationResult.Success(existingPiso, "Piso actualizado correctamente.");
            });
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
    }
}
