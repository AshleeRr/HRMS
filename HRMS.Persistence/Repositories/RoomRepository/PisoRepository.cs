using HRMS.Domain.Base;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Persistence.Repositories.RoomRepository;

public class PisoRepository : BaseRepository<Piso , int> , IPisoRepository
{
    public PisoRepository(HRMSContext context) : base(context)
    {
    }
    
    public async Task<OperationResult> GetByActivoAsync()
    {
        var result = new OperationResult();
        try
        {
            var data = await _Context.Pisos
                .Where(x => x.Estado == true)
                .ToListAsync();
            result.Data = data;
        }
        catch (Exception)
        {
            result.IsSuccess = false;
            result.Message = "Ocurrió un error obteniendo los pisos activos.";
            
        }
        return result;
    }
}