using HRMS.Domain.Base;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Persistence.Repositories.RoomRepository;

public class EstadoHabitacionRepository : BaseRepository<EstadoHabitacion, int>, IEstadoHabitacionRepository
{
    public EstadoHabitacionRepository(HRMSContext context) : base(context)
    {
    }
    
    
    public async Task<OperationResult> GetActivosAsync()
    {
        var result = new OperationResult();
        try
        {
            var datos = await _context.Set<EstadoHabitacion>()
                .Where(e => e.Estado ==  true)
                .ToListAsync();
            result.Data = datos;
        }
        catch (Exception)
        {
            result.IsSuccess = false;
            result.Message = "Ocurrió un error obteniendo los estados activos.";
        }
        return result;
    }
    public async Task<OperationResult> GetByEstadoDescripcionAsync(string descripcionEstado)
    {
        var result = new OperationResult();
        try
        {
            var query = from h in _context.Habitaciones
                join e in _context.EstadoHabitaciones 
                    on h.IdEstadoHabitacion equals e.IdEstadoHabitacion
                where e.Descripcion == descripcionEstado && h.Estado == true
                select h;
                    
            result.Data = await query.ToListAsync();
            result.IsSuccess = true;
        }
        catch (Exception)
        {
            result.IsSuccess = false;
            result.Message = "Ocurrió un error obteniendo habitaciones por estado.";
        }
        return result;
    } 
}