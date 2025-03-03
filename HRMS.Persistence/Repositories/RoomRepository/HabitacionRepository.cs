using System.ComponentModel.DataAnnotations;
using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Base.Validator.RoomValidations;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Base;
using HRMS.Persistence.Interfaces.IRoomRepository;
using HRMS.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HRMS.Persistence.Repositories.RoomRepository;

public class HabitacionRepository : BaseRepository<Habitacion, int>, IHabitacionRepository
{
    private readonly ILogger<HabitacionRepository> _logger;
    private readonly IConfiguration _configuration;
    private  IValidator<Habitacion> _validator;


    public HabitacionRepository(HRMSContext context ,  ILogger<HabitacionRepository> logger,
        IConfiguration configuration ,  IValidator<Habitacion> validator) : base(context)
    {
        _logger = logger;
        _configuration = configuration;
        _validator = validator;
    }

    public override Task<List<Habitacion>> GetAllAsync()
    {
        return _context.Habitaciones.
            Where(h => h.Estado == true)
            .ToListAsync();
    }

    public override async Task<Habitacion> GetEntityByIdAsync(int id)
    {
        if (id != 0)
        {
            return (await _context.Set<Habitacion>().FindAsync(id))!;
        }

        return null;
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
        }catch (Exception)
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

    public override async Task<OperationResult> SaveEntityAsync(Habitacion habitacion)
    {
        var result = new OperationResult();
        try
        {
            var validator = new HabitacionValidator();
            var validation = validator.Validate(habitacion);
            if (!validation.IsSuccess)
            {
                return validation;
            }

            var existingRoom = await _context.Habitaciones
                .FirstOrDefaultAsync(h => h.Numero == habitacion.Numero);

            if (existingRoom != null)
            {
                result.IsSuccess = false;
                result.Message = $"Ya existe una habitación con el número '{habitacion.Numero}'.";
            }

            await _context.Habitaciones.AddAsync(habitacion);
            await _context.SaveChangesAsync();

            result.IsSuccess = true;
            result.Message = "Habitación guardada exitosamente.";
            result.Data = habitacion; 
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.Message = $"Error al guardar la habitación: {ex.Message}";
        }

        return result;
    }

    public override async Task<OperationResult> UpdateEntityAsync(Habitacion habitacion)
    {
        var result = new OperationResult();
        try
        {
            var validator = new HabitacionValidator();
            var validation = validator.Validate(habitacion);
            if (!validation.IsSuccess)
            {
                return validation;
            }

            var existingRoom = await _context.Habitaciones
                .FirstOrDefaultAsync(h => h.IdHabitacion == habitacion.IdHabitacion);

            if (existingRoom == null)
            {
                result.IsSuccess = false;
                result.Message = "La habitación no existe.";
                return result;
            }

            if (habitacion.Numero != existingRoom.Numero)
            {
                var duplicateRoom = await _context.Habitaciones
                    .FirstOrDefaultAsync(
                        h => h.Numero == habitacion.Numero && h.IdHabitacion != habitacion.IdHabitacion);

                if (duplicateRoom != null)
                {
                    result.IsSuccess = false;
                    result.Message = $"Ya existe otra habitación con el número '{habitacion.Numero}'.";
                    return result;
                }
            }

            existingRoom.Numero = habitacion.Numero;
            existingRoom.Detalle = habitacion.Detalle;
            existingRoom.Precio = habitacion.Precio;
            existingRoom.IdPiso = habitacion.IdPiso;
            existingRoom.IdCategoria = habitacion.IdCategoria;
            existingRoom.IdEstadoHabitacion = habitacion.IdEstadoHabitacion;

            await _context.SaveChangesAsync();

            result.IsSuccess = true;
            result.Message = "Habitación actualizada correctamente.";
            result.Data = existingRoom;
        }
        catch (DbUpdateConcurrencyException)
        {
            result.IsSuccess = false;
            result.Message = "La habitación ha sido modificada por otro usuario. Por favor, vuelva a intentarlo.";
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.Message = $"Error al actualizar la habitación: {ex.Message}";
        }

        return result;
    }
}