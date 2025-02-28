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

    public override async Task<List<Piso>> GetAllAsync()
    {
        return  _context.Pisos.Where(p =>
                p.Estado == true)
            .ToList();
    }
    public async Task<OperationResult> GetPisoByDescripcion(string descripcion)
    {
        var result = new OperationResult();
        try
        {
            var query = from p in _context.Pisos
                where p.Descripcion.Contains(descripcion)
                select p;
                   
            var pisos = await query.ToListAsync();
        
            result.Data = pisos;
            result.IsSuccess = true;
        }
        catch (Exception)
        {
            result.IsSuccess = false;
            result.Message = "Ocurrió un error obteniendo el piso por descripción.";
        }
        return result;
    }
}