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
    
    public override async Task<List<Categoria>> GetAllAsync()
    {
        return await _context.Categorias
            .Where(c => c.Estado == true)
            .ToListAsync();
    }

    public async Task<OperationResult> GetByServiciosAsync(string nombre)
    {
        OperationResult result = new OperationResult();
        
        try
        {
            var query = from c in _context.Categorias
                join s in _context.Servicios on c.IdServicio equals s.IdSercicio
                where s.Descripcion.Contains(nombre)
                select c;
            var categorias = await query.ToListAsync();
            result.Data = categorias;
            result.IsSuccess = true;
            return result;
        }
        catch (Exception)
        {
            result.IsSuccess = false;
            result.Message = "Ocurrió un error obteniendo las categorías por servicio.";
            return result;
        }
    }

    public async Task<OperationResult> GetServiciosByDescripcionAsync(string nombre)
    {
        OperationResult result = new OperationResult();
        try
        {
            var query = from c in _context.Categorias
                join s in _context.Servicios on c.IdServicio equals s.IdSercicio
                where s.Descripcion.Contains(nombre)
                select s;
            
            var servicios = await query.ToListAsync();
            result.Data = servicios;
            result.IsSuccess = true;
            
            return result;
        }
        catch (Exception)
        {
            result.IsSuccess = false;
            result.Message = "Ocurrió un error obteniendo los servicios por descripción.";
            return result;
        }
    }

    public Task<OperationResult> GetHabitacionByCapacidad(int capacidad)
    {
        throw new NotImplementedException();
    }
}