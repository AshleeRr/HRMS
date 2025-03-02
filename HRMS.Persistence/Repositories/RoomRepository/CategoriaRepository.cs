using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator.RoomValidations;
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

    public override async Task<OperationResult> SaveEntityAsync(Categoria categoria)
    {
        var result = new OperationResult();
        try
        {
            var validator = new CategoriaValidator();
            var validation = validator.Validate(categoria);
            if (!validation.IsSuccess)
            {
                return validation; 
            }

            bool exists = await ExistsAsync(c => c.Descripcion == categoria.Descripcion);
            if (exists)
            {
                result.IsSuccess = false;
                result.Message = $"Ya existe una categoría con la descripción '{categoria.Descripcion}'.";
                return result;
            }


            await _context.Categorias.AddAsync(categoria);
            await _context.SaveChangesAsync();

            result.IsSuccess = true;
            result.Message = "Categoría guardada correctamente.";
            result.Data = categoria; 
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.Message = $"Ocurrió un error guardando la categoría: {ex.Message}";
        }
        return result;
    }

    public override async Task<OperationResult> UpdateEntityAsync(Categoria categoria)
    {
        var result = new OperationResult();
        try
        {
            // Validar la entidad
            var validator = new CategoriaValidator();
            var validation = validator.Validate(categoria);
            if (!validation.IsSuccess)
            {
                return validation;
            }

            var existingCategoria = await _context.Categorias.FindAsync(categoria.IdCategoria);
            if (existingCategoria == null)
            {
                result.IsSuccess = false;
                result.Message = "La categoría no existe.";
                return result;
            }

            bool exists = await _context.Categorias
                .AnyAsync(c => c.Descripcion == categoria.Descripcion && c.IdCategoria != categoria.IdCategoria);
            if (exists)
            {
                result.IsSuccess = false;
                result.Message = $"Ya existe otra categoría con la descripción '{categoria.Descripcion}'.";
                return result;
            }

            existingCategoria.Descripcion = categoria.Descripcion;
            existingCategoria.IdServicio = categoria.IdServicio;
            existingCategoria.Capacidad = categoria.Capacidad;
            existingCategoria.Estado = categoria.Estado;

            await _context.SaveChangesAsync();

            result.IsSuccess = true;
            result.Message = "Categoría actualizada correctamente.";
            result.Data = existingCategoria;
        }
        catch (DbUpdateConcurrencyException)
        {
            result.IsSuccess = false;
            result.Message = "La categoría fue modificada por otro usuario. Intente nuevamente.";
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.Message = $"Ocurrió un error actualizando la categoría: {ex.Message}";
        }

        return result;
    }

    public async Task<OperationResult> GetCategoriaByServiciosAsync(string nombre)
    {
        var result = new OperationResult();
    
        try
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                result.IsSuccess = false;
                result.Message = "El nombre del servicio no puede estar vacío.";
                return result;
            }

            var query = from c in _context.Categorias
                join s in _context.Servicios on c.IdServicio equals s.IdSercicio
                where s.Descripcion.Contains(nombre)
                select c;
        
            var categorias = await query.ToListAsync();
        
            result.Data = categorias;
            result.IsSuccess = true;
        
            if (!categorias.Any())
            {
                result.Message = $"No se encontraron categorías para el servicio '{nombre}'.";
            }
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.Message = $"Ocurrió un error obteniendo las categorías por servicio: {ex.Message}";
        }
    
        return result;
    }

    public async Task<OperationResult> GetServiciosByDescripcionAsync(string descripcion)
    {
        OperationResult result = new OperationResult();
        try
        {
            if (string.IsNullOrWhiteSpace(descripcion))
            {
                result.IsSuccess = false;
                result.Message = "La descripción del servicio no puede estar vacía.";
                return result;
            }
            
            var query = from c in _context.Categorias
                join s in _context.Servicios on c.IdServicio equals s.IdSercicio
                where s.Descripcion.Contains(descripcion)
                select s;
            
            var servicios = await query.ToListAsync();
            result.Data = servicios;
            result.IsSuccess = true;
            
        }
        catch (Exception)
        {
            result.IsSuccess = false;
            result.Message = "Ocurrió un error obteniendo los servicios por descripción.";
        }

        return result;
    }
    
    public async Task<OperationResult> GetHabitacionByCapacidad(int capacidad)
    {
        var result = new OperationResult();
        try
        {
            if (capacidad <= 0)
            {
                result.IsSuccess = false;
                result.Message = "La capacidad debe ser mayor que cero.";
                return result;
            }

            var query = from c in _context.Categorias
                join h in _context.Habitaciones on c.IdCategoria equals h.IdCategoria
                where c.Capacidad == capacidad && h.Estado == true
                select h;
            
            var habitaciones = await query.ToListAsync();
        
            result.Data = habitaciones;
            result.IsSuccess = true;
        
            if (!habitaciones.Any())
            {
                result.Message = $"No se encontraron habitaciones con capacidad para {capacidad} personas.";
            }
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.Message = $"Ocurrió un error obteniendo las habitaciones por capacidad: {ex.Message}";
        }

        return result;
    }
}