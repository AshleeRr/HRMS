using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator.ServiceValidations;
using HRMS.Domain.Entities.Servicio;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IServicioRepository;
using Microsoft.EntityFrameworkCore;
using MyValidator.Validator;

namespace HRMS.Persistence.Repositories.ServiciosRepository
{
    public class ServicioRepository : BaseRepository<Servicios, int>, IServicioRepository
    {
        public ServicioRepository(HRMSContext context) : base(context) {}
        
        public override async Task<List<Servicios>> GetAllAsync()
        {
            return await _context.Set<Servicios>()
                .Where(s => s.Estado == true)
                .ToListAsync();
        }
        
        public override async Task<OperationResult> SaveEntityAsync(Servicios servicios)
        {
            OperationResult result = new OperationResult();
            try
            {
                var validator = new ServiciosValidator();
                var validation = validator.Validate(servicios);
                if(!validation.IsSuccess)
                {
                    return validation;
                }
                var existe = await _context.Set<Servicios>()
                    .AnyAsync(s => s.Nombre == servicios.Nombre && s.Estado == true);
                if(existe)
                {
                    result.IsSuccess = false;
                    result.Message = "Ya existe un servicio con el mismo nombre.";
                    return result;
                }
                await _context.Set<Servicios>().AddAsync(servicios);
                await _context.SaveChangesAsync();
                
                result.IsSuccess = true;
                result.Message = "Servicio guardado correctamente.";
                result.Data = servicios;
            }catch(Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "Ocurrió un error guardando el servicio.";
            }
            return result;
        }
        
        public override async Task<Servicios> GetEntityByIdAsync(int id)
        {
            if(id != 0)
            {
                return (await _context.Set<Servicios>().FindAsync(id))!;
            }

            return null;
        }

        public override async Task<OperationResult> UpdateEntityAsync(Servicios servicios)
        {
            OperationResult result = new OperationResult();
    
            if (servicios.IdSercicio <= 0)
            {
                result.IsSuccess = false;
                result.Message = "El ID del servicio no es válido";
                return result;
            }
            var servicio = await _context.Set<Servicios>().FindAsync(servicios.IdSercicio);
            if (servicio == null)
            {
                result.IsSuccess = false;
                result.Message = "Servicio no encontrado.";
                return result;
            }
    
            try
            {
                var validator = new ServiciosValidator();
                var validation = validator.Validate(servicios);
                if (!validation.IsSuccess)
                {
                    return validation;
                }
                var existingService = await _context.Set<Servicios>()
                    .FirstOrDefaultAsync(s => s.Nombre == servicios.Nombre && s.IdSercicio != servicios.IdSercicio);
            
                if (existingService != null)
                {
                    result.IsSuccess = false;
                    result.Message = $"Ya existe un servicio con el nombre '{servicios.Nombre}'.";
                    return result;
                }
                servicio.Nombre = servicios.Nombre;
                servicio.Descripcion = servicios.Descripcion;
        
                _context.Set<Servicios>().Update(servicio);
                await _context.SaveChangesAsync();
        
                result.IsSuccess = true;
                result.Message = "Servicio actualizado correctamente.";
                result.Data = servicio;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"Error al actualizar el servicio: {ex.Message}";
            }
    
            return result;
        }

        public async Task<OperationResult> GetServiciosByNombreAsync(string nombre)
        {
            var result = new OperationResult();
            try
            {
                var datos = await _context.Set<Servicios>()
                    .Where(s => s.Nombre.Contains(nombre))
                    .ToListAsync();
                result.Data = datos;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "Ocurrió un error buscando servicios por nombre.";
                
            }
            return result;
        }
    }
}
    

