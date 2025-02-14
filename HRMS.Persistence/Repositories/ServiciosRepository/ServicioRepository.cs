

using HRMS.Domain.Base;
using HRMS.Domain.Entities.Servicio;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IServicioRepository;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Persistence.Repositories.ServiciosRepository
{
    public class ServicioRepository : BaseRepository<Servicios, int>, IServicioRepository
    {
        public ServicioRepository(HRMSContext context) : base(context)
        {
        }

        public async Task<OperationResult> GetAllServiciosAsync()
        {
            var result = new OperationResult();
            try
            {
                var datos = await _Context.Set<Servicios>().ToListAsync();
                result.Data = datos;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "Ocurrió un error obteniendo todos los servicios.";
               
            }
            return result;
        }

        public async Task<OperationResult> GetServicioByIdAsync(short id)
        {
            var result = new OperationResult();
            try
            {
                var dato = await _Context.Set<Servicios>().FindAsync(id);
                result.Data = dato;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "Ocurrió un error obteniendo el servicio por ID.";
               
            }
            return result;
        }

        public async Task<OperationResult> AddServicioAsync(Servicios servicio)
        {
            var result = new OperationResult();
            try
            {
                await _Context.Set<Servicios>().AddAsync(servicio);
                await _Context.SaveChangesAsync();
                result.Data = servicio;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "Ocurrió un error agregando el servicio.";
                
            }
            return result;
        }

        public async Task<OperationResult> UpdateServicioAsync(Servicios servicio)
        {
            var result = new OperationResult();
            try
            {
                _Context.Set<Servicios>().Update(servicio);
                await _Context.SaveChangesAsync();
                result.Data = servicio;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "Ocurrió un error actualizando el servicio.";
                
            }
            return result;
        }

        public async Task<OperationResult> DeleteServicioAsync(short id)
        {
            var result = new OperationResult();
            try
            {
                var servicio = await _Context.Set<Servicios>().FindAsync(id);
                if (servicio != null)
                {
                    _Context.Set<Servicios>().Remove(servicio);
                    await _Context.SaveChangesAsync();
                    result.Data = true;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = "Servicio no encontrado.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "Ocurrió un error eliminando el servicio.";
                
            }
            return result;
        }

        public async Task<OperationResult> GetServiciosByNombreAsync(string nombre)
        {
            var result = new OperationResult();
            try
            {
                var datos = await _Context.Set<Servicios>()
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
    

