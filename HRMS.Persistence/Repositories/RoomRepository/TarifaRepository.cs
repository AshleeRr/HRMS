using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator.ServiceValidations;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Persistence.Repositories.RoomRepository;

public class TarifaRepository : BaseRepository<Tarifas, int> ,  ITarifaRepository
{

    public TarifaRepository(HRMSContext context) : base(context) 
    {
    }

    public override async Task<OperationResult> SaveEntityAsync(Tarifas tarifas)
    {
        var result = new OperationResult();
        try
        {
            var validator = new TarifasValidator();
            var validation = validator.Validate(tarifas);
            if (!validation.IsSuccess)
            {
                return validation;
            }
        
            bool exists = await ExistsAsync(t => 
                t.IdHabitacion == tarifas.IdHabitacion && 
                ((tarifas.FechaInicio <= t.FechaFin && tarifas.FechaFin >= t.FechaInicio) || 
                 (t.FechaInicio <= tarifas.FechaFin && t.FechaFin >= tarifas.FechaInicio)));
             
            if (exists)
            {
                result.IsSuccess = false;
                result.Message = "Ya existe una tarifa para esta habitación y periodo.";
                return result;
            }
        
            bool habitacionExists = await _context.Habitaciones.AnyAsync(h => h.IdHabitacion == tarifas.IdHabitacion);
            if (!habitacionExists)
            {
                result.IsSuccess = false;
                result.Message = "La habitación especificada no existe.";
                return result;
            }
        
            tarifas.FechaCreacion = DateTime.Now;
            tarifas.Estado = true;
        
            await _context.Tarifas.AddAsync(tarifas);
            await _context.SaveChangesAsync();
        
            result.IsSuccess = true;
            result.Message = "Tarifa guardada correctamente.";
            result.Data = tarifas;
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.Message = $"Ocurrió un error guardando la tarifa: {ex.Message}";
        }

        return result;
    }

    public async Task<OperationResult> GetHabitacionByPrecioAsync(decimal precio)
    {
        var result = new OperationResult();

        try
        {
            if (precio <= 0)
            {
                result.IsSuccess = false;
                result.Message = "El precio debe ser mayor que cero.";
                return result;
            }

            var query = from h in _context.Habitaciones
                join t in _context.Tarifas on h.IdHabitacion equals t.IdHabitacion
                where t.PrecioPorNoche == precio && h.Estado == true && t.Estado == true
                select h;
            
            var habitaciones = await query.ToListAsync();
        
            result.Data = habitaciones;
            result.IsSuccess = true;
        
            if (!habitaciones.Any())
            {
                result.Message = $"No se encontraron habitaciones con el precio de {precio:C}.";
            }
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.Message = $"Ocurrió un error obteniendo la habitación por precio: {ex.Message}";
        }
    
        return result;
    }

    public override async Task<OperationResult> UpdateEntityAsync(Tarifas tarifas)
    {
        var result = new OperationResult();
        try
        {
            var validator = new TarifasValidator();
            var validation = validator.Validate(tarifas);
            if (!validation.IsSuccess)
            {
                return validation;
            }
            
            var existingTarifa = await _context.Tarifas.FindAsync(tarifas.IdTarifa);
            if (existingTarifa == null)
            {
                result.IsSuccess = false;
                result.Message = "La tarifa no existe.";
                return result;
            }

            if (existingTarifa.IdHabitacion != tarifas.IdHabitacion ||
                existingTarifa.FechaInicio != tarifas.FechaInicio ||
                existingTarifa.FechaFin != tarifas.FechaFin)
            {
                bool exists = await _context.Tarifas
                    .AnyAsync(t =>
                        t.IdTarifa != tarifas.IdTarifa &&
                        t.IdHabitacion == tarifas.IdHabitacion &&
                        ((tarifas.FechaInicio <= t.FechaFin && tarifas.FechaFin >= t.FechaInicio) ||
                         (t.FechaInicio <= tarifas.FechaFin && t.FechaFin >= tarifas.FechaInicio)));

                if (exists)
                {
                    result.IsSuccess = false;
                    result.Message = "Ya existe una tarifa para esta habitación y periodo.";
                    return result;
                }
            }
            
            if (existingTarifa.IdHabitacion != tarifas.IdHabitacion)
            {
                bool habitacionExists =
                    await _context.Habitaciones.AnyAsync(h => h.IdHabitacion == tarifas.IdHabitacion);
                if (!habitacionExists)
                {
                    result.IsSuccess = false;
                    result.Message = "La habitación especificada no existe.";
                    return result;
                }
            }

            existingTarifa.IdHabitacion = tarifas.IdHabitacion;
            existingTarifa.FechaInicio = tarifas.FechaInicio;
            existingTarifa.FechaFin = tarifas.FechaFin;
            existingTarifa.PrecioPorNoche = tarifas.PrecioPorNoche;
            existingTarifa.Descuento = tarifas.Descuento;
            existingTarifa.Descripcion = tarifas.Descripcion;
            existingTarifa.Estado = tarifas.Estado;

            await _context.SaveChangesAsync();

            result.IsSuccess = true;
            result.Message = "Tarifa actualizada correctamente.";
            result.Data = existingTarifa;
        }
        catch (DbUpdateConcurrencyException)
        {
            result.IsSuccess = false;
            result.Message = "La tarifa fue modificada por otro usuario. Intente nuevamente.";
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.Message = $"Ocurrió un error actualizando la tarifa: {ex.Message}";
        }

        return result;
    }
    
    public override async Task<List<Tarifas>> GetAllAsync()
    {
        return await _context.Tarifas.Where(t => t.Estado == true).ToListAsync();
    }

    public async Task<OperationResult> GetTarifasVigentesAsync(DateTime fecha)
    {
        var result = new OperationResult();
        try
        {
            var tarifas = await _context.Tarifas
                .Where(t => t.FechaInicio <= fecha && t.FechaFin >= fecha && t.Estado == true)
                .ToListAsync();
            
            result.Data = tarifas;
            result.IsSuccess = true;
        
            if (!tarifas.Any())
            {
                result.Message = $"No se encontraron tarifas vigentes para la fecha {fecha:d}.";
            }
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.Message = $"Ocurrió un error obteniendo las tarifas vigentes: {ex.Message}";
        }
    
        return result;
    }
    
    public override async Task<Tarifas> GetEntityByIdAsync(int id)
    {
        if (id != 0)
        {
            return(await _context.Tarifas.FindAsync(id))!; 
        }
        return null;
    }
}