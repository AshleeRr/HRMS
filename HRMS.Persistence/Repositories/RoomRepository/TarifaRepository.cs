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

    public async override Task<OperationResult> SaveEntityAsync(Tarifas tarifas)
    {
        OperationResult result = new OperationResult();
        try
        {
            var validator = new TarifasValidator();
            var validation = validator.Validate(tarifas);
            if (!validation.IsSuccess)
            {
                result.IsSuccess = false;
                result.Message = "Datos incorrectos.";
            }
            var exists = ExistsAsync(t => t.IdHabitacion == tarifas.IdHabitacion && t.FechaInicio == tarifas.FechaInicio);
            if (exists.Result)
            {
                result.IsSuccess = false;
                result.Message = "La tarifa ya existe.";
            }
            _context.Tarifas.Add(tarifas);
            _context.SaveChanges();
            result.IsSuccess = true;
            result.Message = "Tarifa guardada correctamente.";
        }
        catch (Exception)
        {
            result.IsSuccess = false;
            result.Message = "Ocurrió un error guardando la tarifa.";
        }

        return result;
    }

    public async Task<OperationResult> GetHabitacionByPrecioAsync(decimal precio)
    {
        var result = new OperationResult();

        try
        {
            var query = from h in _context.Habitaciones
                join t in _context.Tarifas on h.IdHabitacion equals t.IdHabitacion
                where t.PrecioPorNoche == precio
                select h;
            var habitacion = await query.ToListAsync();
            result.Data = habitacion;
            result.IsSuccess = true;
            return result;
        }
        
        catch (Exception)
        {
            result.IsSuccess = false;
            result.Message = "Ocurrió un error obteniendo la habitación por precio.";
            return result;
        }
    }
    
    public override async Task<OperationResult> UpdateEntityAsync(Tarifas tarifas)
    {
        OperationResult result = new OperationResult();
        try
        {
            var validator = new TarifasValidator();
            var validation = validator.Validate(tarifas);
            if (!validation.IsSuccess)
            {
                result.IsSuccess = false;
                result.Message = "Datos incorrectos.";
            }
            var exists = await ExistsAsync(t => t.IdHabitacion == tarifas.IdHabitacion && t.FechaInicio == tarifas.FechaInicio);
            if (!exists)
            {
                result.IsSuccess = false;
                result.Message = "La tarifa no existe.";
            }
            _context.Tarifas.Update(tarifas);
            await _context.SaveChangesAsync();
            result.IsSuccess = true;
            result.Message = "Tarifa actualizada correctamente.";
        }
        catch (Exception)
        {
            result.IsSuccess = false;
            result.Message = "Ocurrió un error actualizando la tarifa.";
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
            var datos = await _context.Set<Tarifas>()
                .Where(t => t.FechaInicio <= fecha && t.FechaFin >= fecha)
                .ToListAsync();
            result.Data = datos;
        }
        catch (Exception)
        {
            result.IsSuccess = false;
            result.Message = "Ocurrió un error obteniendo las tarifas vigentes.";
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