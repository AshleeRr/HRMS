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
            var datos = await _Context.Set<EstadoHabitacion>()
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
}