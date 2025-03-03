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

public class PisoRepository : BaseRepository<Piso, int>, IPisoRepository
{
    private readonly ILogger<PisoRepository> _logger;
    private readonly IConfiguration _configuration;
    private  IValidator<Piso> _validator;


    public PisoRepository(HRMSContext context ,  ILogger<PisoRepository> logger,
        IConfiguration configuration ,  IValidator<Piso> validator) : base(context)
    {
        _logger = logger;
        _configuration = configuration;
        _validator = validator;
    }


    public override async Task<List<Piso>> GetAllAsync()
    {
        return _context.Pisos.Where(p =>
                p.Estado == true)
            .ToList();
    }


    public override async Task<Piso> GetEntityByIdAsync(int id)
    {
        if (id != 0)
        {
            return await _context.Set<Piso>().FindAsync(id);
        }

        return null;
    }

    public override async Task<OperationResult> UpdateEntityAsync(Piso piso)
    {
        var result = new OperationResult();
        try
        {
            var validator = new PisoValidator();
            var validation = validator.Validate(piso);
            if (!validation.IsSuccess)
            {
                return validation;
            }

            var existingPiso = await _context.Pisos.FindAsync(piso.IdPiso);
            if (existingPiso == null)
            {
                result.IsSuccess = false;
                result.Message = "El piso no existe.";
                return result;
            }

            bool exists = await _context.Pisos
                .AnyAsync(p => p.Descripcion == piso.Descripcion && p.IdPiso != piso.IdPiso);
            if (exists)
            {
                result.IsSuccess = false;
                result.Message = $"Ya existe otro piso con la descripción '{piso.Descripcion}'.";
                return result;
            }

            existingPiso.Descripcion = piso.Descripcion;
            existingPiso.Estado = piso.Estado;

            await _context.SaveChangesAsync();

            result.IsSuccess = true;
            result.Message = "Piso actualizado correctamente.";
            result.Data = existingPiso;
        }
        catch (DbUpdateConcurrencyException)
        {
            result.IsSuccess = false;
            result.Message = "El piso fue modificado por otro usuario. Intente nuevamente.";
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.Message = $"Ocurrió un error actualizando el piso: {ex.Message}";
        }

        return result;
    }

    public async Task<OperationResult> GetPisoByDescripcion(string descripcion)
    {
        var result = new OperationResult();
        try
        {
            if (string.IsNullOrWhiteSpace(descripcion))
            {
                result.IsSuccess = false;
                result.Message = "La descripción del piso no puede estar vacía.";
                return result;
            }

            var pisos = await _context.Pisos
                .Where(p => p.Descripcion.Contains(descripcion) && p.Estado == true)
                .ToListAsync();

            result.Data = pisos;
            result.IsSuccess = true;

            if (!pisos.Any())
            {
                result.Message = $"No se encontraron pisos con la descripción '{descripcion}'.";
            }
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.Message = $"Ocurrió un error obteniendo el piso por descripción: {ex.Message}";
        }

        return result;
    }
}