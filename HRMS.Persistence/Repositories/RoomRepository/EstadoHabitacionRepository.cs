using System.ComponentModel.DataAnnotations;
using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Base.Validator.RoomValidations;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HRMS.Persistence.Repositories.RoomRepository;

public class EstadoHabitacionRepository : BaseRepository<EstadoHabitacion, int>, IEstadoHabitacionRepository
{
    private readonly ILogger<EstadoHabitacionRepository> _logger;
    private readonly IConfiguration _configuration;
    private  IValidator<EstadoHabitacion> _validator;


    public EstadoHabitacionRepository(HRMSContext context ,  ILogger<EstadoHabitacionRepository> logger,
        IConfiguration configuration ,  IValidator<EstadoHabitacion> validator) : base(context)
    {
        _logger = logger;
        _configuration = configuration;
        _validator = validator;
    }

    public override async Task<EstadoHabitacion> GetEntityByIdAsync(int id)
    {
        if (id != 0)
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

    public override async Task<OperationResult> SaveEntityAsync(EstadoHabitacion estadoHabitacion)
    {
        var result = new OperationResult();
        try
        {
            var validator = new EstadoHabitacionValidator();
            var validation = validator.Validate(estadoHabitacion);
            if (!validation.IsSuccess)
            {
                return validation;
            }

            bool exists = await ExistsAsync(e => e.Descripcion == estadoHabitacion.Descripcion);
            if (exists)
            {
                result.IsSuccess = false;
                result.Message =
                    $"Ya existe un estado de habitación con la descripción '{estadoHabitacion.Descripcion}'.";
                return result;
            }
            
            await _context.EstadoHabitaciones.AddAsync(estadoHabitacion);
            await _context.SaveChangesAsync();

            result.IsSuccess = true;
            result.Message = "Estado de habitación guardado correctamente.";
            result.Data = estadoHabitacion;
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.Message = $"Ocurrió un error guardando el estado de habitación: {ex.Message}";
        }

        return result;
    }

    public async Task<OperationResult> GetEstadoByDescripcionAsync(string descripcionEstado)
    {
        var result = new OperationResult();
        try
        {
            if (string.IsNullOrWhiteSpace(descripcionEstado))
            {
                result.IsSuccess = false;
                result.Message = "La descripción del estado no puede estar vacía.";
                return result;
            }

            var estado = await _context.EstadoHabitaciones
                .FirstOrDefaultAsync(e => e.Descripcion == descripcionEstado && e.Estado == true);

            if (estado == null)
            {
                result.IsSuccess = false;
                result.Message = $"No se encontró un estado de habitación con la descripción '{descripcionEstado}'.";
                return result;
            }

            result.Data = estado;
            result.IsSuccess = true;
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.Message = $"Ocurrió un error obteniendo el estado de habitación: {ex.Message}";
        }

        return result;
    }

    public override async Task<OperationResult> UpdateEntityAsync(EstadoHabitacion estadoHabitacion)
    {
        var result = new OperationResult();
        try
        {
            var validator = new EstadoHabitacionValidator();
            var validation = validator.Validate(estadoHabitacion);
            if (!validation.IsSuccess)
            {
                return validation;
            }

            var existingEstado = await _context.EstadoHabitaciones.FindAsync(estadoHabitacion.IdEstadoHabitacion);
            if (existingEstado == null)
            {
                result.IsSuccess = false;
                result.Message = "El estado de habitación no existe.";
                return result;
            }

            var duplicateEstado = await _context.EstadoHabitaciones
                .FirstOrDefaultAsync(e => e.Descripcion == estadoHabitacion.Descripcion &&
                                          e.IdEstadoHabitacion != estadoHabitacion.IdEstadoHabitacion);

            if (duplicateEstado != null)
            {
                result.IsSuccess = false;
                result.Message =
                    $"Ya existe un estado de habitación con la descripción '{estadoHabitacion.Descripcion}'.";
                return result;
            }

            existingEstado.Descripcion = estadoHabitacion.Descripcion;
            existingEstado.Estado = estadoHabitacion.Estado;

            await _context.SaveChangesAsync();

            result.IsSuccess = true;
            result.Message = "Estado de habitación actualizado correctamente.";
            result.Data = existingEstado;
        }
        catch (DbUpdateConcurrencyException)
        {
            result.IsSuccess = false;
            result.Message = "El estado de habitación fue modificado por otro usuario. Intente nuevamente.";
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.Message = $"Ocurrió un error actualizando el estado de habitación: {ex.Message}";
        }

        return result;
    }
}