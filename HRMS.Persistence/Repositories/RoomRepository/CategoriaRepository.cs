using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator.RoomValidations;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.EntityFrameworkCore;
using MyValidator.Validator;

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

    public override async Task<OperationResult> SaveEntityAsync(Categoria categoria)
    {
        OperationResult result = new OperationResult();
        try
        {
            var validator = new CategoriaValidator();
            var validation = validator.Validate(categoria);
            if (!validation.IsSuccess)
            {
                result.IsSuccess = false;
                result.Message = "Datos incorrectos.";
                return result;
            }
            var exists = await ExistsAsync(c => c.Descripcion == categoria.Descripcion);
            if (exists)
            {
                result.IsSuccess = false;
                result.Message = "La categoría ya existe.";
                return result;
            }
            await _context.Categorias.AddAsync(categoria);
            await _context.SaveChangesAsync();
            result.IsSuccess = true;
            result.Message = "Categoría guardada correctamente.";
        }
        catch (Exception)
        {
            result.IsSuccess = false;
            result.Message = "Ocurrió un error guardando la categoría.";
        }

        return result;
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

    public async Task<OperationResult> GetServiciosByDescripcionAsync(string descripcion)
    {
        OperationResult result = new OperationResult();
        try
        {
            var query = from c in _context.Categorias
                join s in _context.Servicios on c.IdServicio equals s.IdSercicio
                where s.Descripcion.Contains(descripcion)
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
    
    public  async Task<OperationResult> GetHabitacionByCapacidad(int capacidad)
    {
        OperationResult result = new OperationResult();
        try
        {
            var query = from c in _context.Categorias
                join h in _context.Habitaciones on c.IdCategoria equals h.IdCategoria
                where c.Capacidad == capacidad
                select h;
            var habitaciones = await query.ToListAsync();
            result.Data = habitaciones;
            result.IsSuccess = true;
            return result;
        }
        catch (Exception)
        {
            result.IsSuccess = false;
            result.Message = "Ocurrió un error obteniendo las habitaciones por capacidad.";
            return result;
        }
    }
}