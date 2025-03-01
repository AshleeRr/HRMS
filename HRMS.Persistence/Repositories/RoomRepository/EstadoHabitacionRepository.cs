using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator.RoomValidations;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Persistence.Repositories.RoomRepository;

public class EstadoHabitacionRepository : BaseRepository<EstadoHabitacion, int>, IEstadoHabitacionRepository
{
    public EstadoHabitacionRepository(HRMSContext context) : base(context)
    {
    }

    public override async Task<EstadoHabitacion> GetEntityByIdAsync(int id)
    {
        if(id != 0)
        {
            return (await _context.Set<EstadoHabitacion>().FindAsync(id))!;
        }

        return null;
    }

    public override Task<List<EstadoHabitacion>> GetAllAsync()
    {
        return _context.EstadoHabitaciones
            .Where(e => e.Estado == true)
            .ToListAsync();
    }

    public override async Task<OperationResult> SaveEntityAsync(EstadoHabitacion entity)
    {
        OperationResult result = new OperationResult();
        try
        {
            var validator = new EstadoHabitacionValidator();
            var validation = validator.Validate(entity);
            if (!validation.IsSuccess)
            {
                result.IsSuccess = false;
                result.Message = "Datos incorrectos.";
            }
            
            var exists = await ExistsAsync(e => e.Descripcion == entity.Descripcion);
            if (exists)
            {
                result.IsSuccess = false;
                result.Message = "El estado de habitación ya existe.";
            }
            
            await _context.EstadoHabitaciones.AddAsync(entity);
            await _context.SaveChangesAsync();
            result.IsSuccess = true;
            result.Message = "Estado de habitación guardado correctamente.";
        }
        catch (Exception)
        {
            result.IsSuccess = false;
            result.Message = "Ocurrió un error guardando el estado de habitación.";
        }

        return result;
    }

    public async Task<OperationResult> GetEstadoByDescripcionAsync(string descripcionEstado)
    {
        OperationResult result = new OperationResult();
        try
        {
            var estado = await _context.EstadoHabitaciones
                .FirstOrDefaultAsync(e => e.Descripcion == descripcionEstado);
            if (estado == null)
            {
                result.IsSuccess = false;
                result.Message = "No se encontró el estado de habitación.";
            }
            result.Data = estado;
            result.IsSuccess = true;
        }
        catch (Exception)
        {
            result.IsSuccess = false;
            result.Message = "Ocurrió un error obteniendo el estado de habitación.";
        }

        return result;
    }

    public override async Task<OperationResult> UpdateEntityAsync(EstadoHabitacion estadoHabitacion)
    {
        OperationResult result = new OperationResult();
        try
        {
            var validator = new EstadoHabitacionValidator();
            var validation = validator.Validate(estadoHabitacion);
            if (!validation.IsSuccess)
            {
                result.IsSuccess = false;
                result.Message = "Datos incorrectos.";
            }
            var existingEstado = _context.EstadoHabitaciones
                .FirstOrDefault(e => e.Descripcion == estadoHabitacion.Descripcion && e.IdEstadoHabitacion != estadoHabitacion.IdEstadoHabitacion);
            if (existingEstado != null)
            {
                result.IsSuccess = false;
                result.Message = "Ya existe un estado de habitación con el mismo nombre.";
            }
            _context.EstadoHabitaciones.Update(estadoHabitacion);
            _context.SaveChanges();
            result.IsSuccess = true;
            result.Message = "Estado de habitación actualizado correctamente.";
        }
        catch (Exception)
        {
            result.IsSuccess = false;
            result.Message = "Ocurrió un error actualizando el estado de habitación.";
        }

        return result;
    }
}