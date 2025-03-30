using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Base.Validator.ServiceValidations;
using HRMS.Domain.Entities.Servicio;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IServicioRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HRMS.Persistence.Repositories.ServiciosRepository;

public class ServicioRepository : BaseRepository<Servicios, short>, IServicioRepository
{
    private readonly ILogger<ServicioRepository> _logger;
    private readonly IConfiguration _configuration;
    private readonly IValidator<Servicios> _validator;
    public ServicioRepository(HRMSContext context, ILogger<ServicioRepository> logger , 
        IConfiguration configuration , IValidator<Servicios> validator) : base(context) 
    {
        _logger = logger;
        _configuration = configuration;
        _validator = validator;
    }
    
    public override async Task<List<Servicios>> GetAllAsync()
    {
        return await _context.Set<Servicios>()
            .Where(s => s.Estado == true)
            .ToListAsync();
    }
    
    public override async Task<OperationResult> SaveEntityAsync(Servicios servicios)
    {
        var result = new OperationResult();
        try
        {
            var validator = new ServiciosValidator();
            var validation = validator.Validate(servicios);
            if (!validation.IsSuccess)
            {
                return validation;
            }

            var existe = await _context.Set<Servicios>()
                .AnyAsync(s => s.Nombre == servicios.Nombre && s.Estado == true);
            if (existe)
            {
                result.IsSuccess = false;
                result.Message = $"Ya existe un servicio con el nombre '{servicios.Nombre}'.";
                return result;
            }
            

            await _context.Set<Servicios>().AddAsync(servicios);
            await _context.SaveChangesAsync();
            
            result.IsSuccess = true;
            result.Message = "Servicio guardado correctamente.";
            result.Data = servicios;
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.Message = $"Ocurrió un error guardando el servicio: {ex.Message}";
            _logger?.LogError(ex, "Error guardando servicio {Nombre}", servicios.Nombre);
        }
        return result;
    }
    
    public override async Task<Servicios> GetEntityByIdAsync(short id)
    {
        _logger.LogInformation("Obteniendo servicio por ID {Id}", id);
        return await _context.Set<Servicios>()
            .FirstOrDefaultAsync(s => s.IdServicio == id && s.Estado == true);
    }

    public override async Task<OperationResult> UpdateEntityAsync(Servicios servicios)
    {
        var result = new OperationResult();
        
        if (servicios.IdServicio <= 0)
        {
            result.IsSuccess = false;
            result.Message = "El ID del servicio no es válido";
            return result;
        }
        
        try
        {
            var servicio = await _context.Set<Servicios>().FindAsync(servicios.IdServicio);
            if (servicio == null)
            {
                result.IsSuccess = false;
                result.Message = "Servicio no encontrado.";
                return result;
            }
            
            var validator = new ServiciosValidator();
            var validation = validator.Validate(servicios);
            if (!validation.IsSuccess)
            {
                return validation;
            }
            
            var existingService = await _context.Set<Servicios>()
                .FirstOrDefaultAsync(s => s.Nombre == servicios.Nombre && 
                                          s.IdServicio != servicios.IdServicio &&
                                          s.Estado == true);
            
            if (existingService != null)
            {
                result.IsSuccess = false;
                result.Message = $"Ya existe un servicio con el nombre '{servicios.Nombre}'.";
                return result;
            }
            
            servicio.Nombre = servicios.Nombre;
            servicio.Descripcion = servicios.Descripcion;
            servicio.Estado = servicios.Estado;
            
            await _context.SaveChangesAsync();
            
            result.IsSuccess = true;
            result.Message = "Servicio actualizado correctamente.";
            result.Data = servicio;
        }
        catch (DbUpdateConcurrencyException)
        {
            result.IsSuccess = false;
            result.Message = "El servicio fue modificado por otro usuario. Intente nuevamente.";
            _logger?.LogWarning("Conflicto de concurrencia actualizando servicio ID {Id}", servicios.IdServicio);
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.Message = $"Error al actualizar el servicio: {ex.Message}";
            _logger?.LogError(ex, "Error actualizando servicio ID {Id}", servicios.IdServicio);
        }
        
        return result;
    }

    public async Task<OperationResult> GetServiciosByNombreAsync(string nombre)
    {
        var result = new OperationResult();
        try
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                result.IsSuccess = false;
                result.Message = "El nombre de búsqueda no puede estar vacío.";
                return result;
            }

            var servicios = await _context.Set<Servicios>()
                .Where(s => s.Nombre.Contains(nombre) && s.Estado == true)
                .ToListAsync();

            result.Data = servicios;
            result.IsSuccess = true;

            if (!servicios.Any())
            {
                result.Message = $"No se encontraron servicios con el nombre '{nombre}'.";
            }
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.Message = $"Ocurrió un error buscando servicios por nombre: {ex.Message}";
            _logger?.LogError(ex, "Error buscando servicios por nombre {Nombre}", nombre);
        }

        return result;
    }
    
    private static OperationResult ValidateInt(int value, string fieldName)
    {
        var result = new OperationResult();
        if (value <= 0)
        {
            result.IsSuccess = false;
            result.Message = $"El campo {fieldName} debe ser mayor que cero.";
        }
        return result;
    }
}