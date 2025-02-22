using HRMS.Domain.Base;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Base;
using HRMS.Persistence.Interfaces.IRoomRepository;
using HRMS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Persistence.Repositories.RoomRepository;

public class HabitacionRepository : BaseRepository<Habitacion, int>, IHabitacionRepository
{
    public HabitacionRepository(HRMSContext context) : base(context) {}
    public async Task<OperationResult> GetByEstadoAsync(bool estado)
    {
        var result = new OperationResult();
        try
        {
            var datos = await _context.Set<Habitacion>()
                .Where(h => h.Estado == estado)
                .ToListAsync(); 
            result.Data = datos;
        }
        catch (Exception)
        {
            result.IsSuccess = false;
            result.Message = "Ocurrió un error obteniendo las habitaciones por estado.";
        }
        return result;
    }

    public async Task<OperationResult> GetByPisoAsync(int idPiso)
    {
        var result = new OperationResult();
        try
        {
            var datos = await _context.Set<Habitacion>()
                .Where(h => h.IdPiso == idPiso)
                .ToListAsync();
            result.Data = datos;
        }
        catch (Exception)
        {
            result.IsSuccess = false;
            result.Message = "Ocurrió un error obteniendo las habitaciones por piso.";
        }
        return result;
    }

    public async Task<OperationResult> GetByCategoriaAsync(int idCategoria)
    {
        var result = new OperationResult();
        try
        {
            var datos = await _context.Set<Habitacion>()
                .Where(h => h.IdCategoria == idCategoria)
                .ToListAsync();
            result.Data = datos;
        }
        catch (Exception)
        {
            result.IsSuccess = false;
            result.Message = "Ocurrió un error obteniendo las habitaciones por categoría.";
        }
        return result;
    }

    public async Task<OperationResult> GetByNumeroAsync(string numero)
    {
        var result = new OperationResult();
        try
        {
            var dato = await _context.Set<Habitacion>()
                .FirstOrDefaultAsync(h => h.Numero == numero);
            result.Data = dato;
        }
        catch (Exception)
        {
            result.IsSuccess = false;
            result.Message = "Ocurrió un error obteniendo la habitación por número.";
        }
        return result;
    }
}