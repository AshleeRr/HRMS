using HRMS.Domain.Base;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Persistence.Repositories.RoomRepository
{
    public class PisoRepository : BaseRepository<Piso, int>, IPisoRepository
    {
        public PisoRepository(HRMSContext context) : base(context)
        {
        }

        public override async Task<List<Piso>> GetAllAsync()
        {
            return await _context.Pisos.Where(p => p.Estado == true).ToListAsync();
        }
        
        public override async Task<OperationResult> SaveEntityAsync(Piso piso)
        {
            try
            {
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
            return (id == 0 ? null : await _context.Pisos
                .FirstOrDefaultAsync(p => p.IdPiso == id && p.Estado == true))!;
        }

        public async Task<OperationResult> GetPisoByDescripcion(string descripcion)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
            
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
    }
}