using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator.RoomValidations;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Persistence.Repositories.RoomRepository;

public class PisoRepository : BaseRepository<Piso , int> , IPisoRepository
{
    public PisoRepository(HRMSContext context) : base(context)
    {
    }

    public override async Task<List<Piso>> GetAllAsync()
    {
        return  _context.Pisos.Where(p =>
                p.Estado == true)
            .ToList();
    }

    public override  async Task<OperationResult> SaveEntityAsync(Piso piso)
    {
        OperationResult result = new OperationResult();
        try
        {
            var validator = new PisoValidator();
            var validation = validator.Validate(piso);
            if (!validation.IsSuccess)
            {
                result.IsSuccess = false;
                result.Message = "Datos incorrectos.";
                return result;
            }

            var exists = ExistsAsync(p => p.Descripcion == piso.Descripcion);
            if (exists.Result)
            {
                result.IsSuccess = false;
                result.Message = "El piso ya existe.";
                return result;
            }

            _context.Pisos.Add(piso);
            _context.SaveChanges();
            result.IsSuccess = true;
            result.Message = "Piso guardado correctamente.";
        }
        catch (Exception)
        {
            result.IsSuccess = false;
            result.Message = "Ocurrió un error guardando el piso.";
        }
        return result;
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
        OperationResult result = new OperationResult();
        try
        {
            var validator = new PisoValidator();
            var validation = validator.Validate(piso);
            if (!validation.IsSuccess)
            {
                return validation;
            }

            var exists = ExistsAsync(p => p.Descripcion == piso.Descripcion);
            if (exists.Result)
            {
                result.IsSuccess = false;
                result.Message = "El piso ya existe.";
                return result;
            }

            _context.Pisos.Update(piso);
            _context.SaveChanges();
            result.IsSuccess = true;
            result.Message = "Piso actualizado correctamente.";
        }
        catch (Exception)
        {
            result.IsSuccess = false;
            result.Message = "Ocurrió un error actualizando el piso.";
        }

        return result;
    }

    public async Task<OperationResult> GetPisoByDescripcion(string descripcion)
    {
        var result = new OperationResult();
        try
        {
            var query = from p in _context.Pisos
                where p.Descripcion.Contains(descripcion)
                select p;
                   
            var pisos = await query.ToListAsync();
        
            result.Data = pisos;
            result.IsSuccess = true;
        }
        catch (Exception)
        {
            result.IsSuccess = false;
            result.Message = "Ocurrió un error obteniendo el piso por descripción.";
        }
        return result;
    }
}