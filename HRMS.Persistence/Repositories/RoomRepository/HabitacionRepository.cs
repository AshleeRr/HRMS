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

    public override Task<List<Habitacion>> GetAllAsync()
    {
        return _context.Habitaciones.
            Where(h => h.Estado == true)
            .ToListAsync();
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

    public async Task<OperationResult> GetInfoHabitacionesAsync()
    {
        var result = new OperationResult();
        try
        {
            var query = from h in _context.Habitaciones
                join p in _context.Pisos on h.IdPiso equals p.IdPiso
                join c in _context.Categorias on h.IdCategoria equals c.IdCategoria
                join t in _context.Tarifas on h.IdHabitacion equals t.IdHabitacion
                where h.Estado == true && (t.FechaInicio <= DateTime.Now && t.FechaFin >= DateTime.Now)
                select new
                {
                    h.IdHabitacion,
                    h.Numero,
                    h.Detalle,
                    h.Estado,
                    t.PrecioPorNoche,
                    DescripcionPiso = p.Descripcion,
                    DescripcionCategoria = c.Descripcion,
                };
            
            var habitaciones = await query.ToListAsync();
            result.Data = habitaciones;
            result.IsSuccess = true;
        
            return result;
        }
        catch (Exception e)
        {
            result.IsSuccess = false;
            result.Message = "Ocurrió un error obteniendo la información de las habitaciones.";
            return result;
        }
    }

    public async Task<OperationResult> GetByCategoriaAsync(string categoria)
    {
        OperationResult result = new OperationResult();
        try
        {
           var query = from h in _context.Habitaciones
               join c in _context.Categorias on h.IdCategoria equals c.IdCategoria
               where c.Descripcion.Contains(categoria)
               select h; 
           var habitaciones = query.ToListAsync();
            result.Data = habitaciones; 
            result.IsSuccess = true;
            return result;
        }catch (Exception)
        {
            result.IsSuccess = false;
            result.Message = "Ocurrió un error obteniendo las habitaciones por categoría.";
            return result;
        }
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

    public override async Task<OperationResult> SaveEntityAsync(Habitacion entity)
    {
        var result = new OperationResult();
        try
        {
            if (string.IsNullOrEmpty(entity.Numero))
            {
                result.IsSuccess = false;
                result.Message = "El número de habitación es requerido.";
                return result;
            }
            var existingRoom = await GetByNumeroAsync(entity.Numero);
            
            if (((OperationResult)existingRoom).Data != null)
            {
                result.IsSuccess = false;
                result.Message = "Ya existe una habitación con ese número.";
                return result;
            }
            entity.FechaCreacion = DateTime.Now;
            entity.Estado = true;

            await _context.Habitaciones.AddAsync(entity);
            await _context.SaveChangesAsync();

            result.Data = entity;
            result.IsSuccess = true;
            result.Message = "Habitación creada exitosamente.";
        }
        catch (Exception)
        {
            result.IsSuccess = false;
            result.Message = "Ocurrió un error guardando la habitación.";
        }

        return result;
    }
    public override async Task<OperationResult> UpdateEntityAsync(Habitacion entity)
    {
        var result = new OperationResult();
        try
        {
            var habitacion = await _context.Habitaciones.FindAsync(entity.IdHabitacion);
            if (habitacion == null)
            {
                result.IsSuccess = false;
                result.Message = "Habitación no encontrada.";
                return result;
            }

            if (!string.IsNullOrEmpty(entity.Numero)) habitacion.Numero = entity.Numero;
            if (!string.IsNullOrEmpty(entity.Detalle)) habitacion.Detalle = entity.Detalle;
            if (entity.IdEstadoHabitacion.HasValue) habitacion.IdEstadoHabitacion = entity.IdEstadoHabitacion;
            if (entity.IdPiso.HasValue) habitacion.IdPiso = entity.IdPiso;
            if (entity.IdCategoria.HasValue) habitacion.IdCategoria = entity.IdCategoria;
            if (entity.Estado.HasValue) habitacion.Estado = entity.Estado;

            await _context.SaveChangesAsync();
        
            result.IsSuccess = true;
            result.Message = "Habitación actualizada exitosamente.";
            result.Data = habitacion;
        }     
        catch (Exception)
        {
            result.IsSuccess = false;
            result.Message = "Error al actualizar la habitación.";
        }
        return result;
    }
}