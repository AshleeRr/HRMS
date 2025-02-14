using HRMS.Domain.Base;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Persistence.Repositories.RoomRepository;

public class CategoriaRepository : BaseRepository<Categoria, int>, ICategoryRepository
{
    public CategoriaRepository(HRMSContext context) : base(context) { }

    public async Task<OperationResult> GetByServiciosAsync(int idServicio)
    {
        var result = new OperationResult();
        try
        {
            var datos = await _context.Set<Categoria>()
                .Where(c => c.IdServicio == idServicio)
                .ToListAsync();
            result.Data = datos;
        }
        catch (Exception)
        {
            result.IsSuccess = false;
            result.Message = "Ocurrió un error obteniendo las categorías por servicio.";
        }
        return result;
    }

    public async Task<OperationResult> GetActivasAsync()
    {
        var result = new OperationResult();
        try
        {
            var datos = await _context.Set<Categoria>()
                .Where(c => c.Estado == true)
                .ToListAsync();
            result.Data = datos;
        }
        catch (Exception)
        {
            result.IsSuccess = false;
            result.Message = "Ocurrió un error obteniendo las categorías activas.";
        }
        return result;
    }
}