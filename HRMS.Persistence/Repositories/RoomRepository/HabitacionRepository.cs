using HRMS.Domain.Base;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Domain.Entities.Servicio;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HRMS.Persistence.Repositories.RoomRepository
{
    public class HabitacionRepository : BaseRepository<Habitacion, int>, IHabitacionRepository
    {
        private readonly ILogger<HabitacionRepository> _logger;

        public HabitacionRepository(HRMSContext context, ILogger<HabitacionRepository> logger) 
            : base(context)
        {
            _logger = logger;
        }

        public override async Task<List<Habitacion>> GetAllAsync() =>
            await _context.Habitaciones.Where(h => h.Estado == true).ToListAsync();

        public override async Task<Habitacion> GetEntityByIdAsync(int id)
        {
            return (id == 0 ? null : await _context.Habitaciones
                .FirstOrDefaultAsync(h => h.IdHabitacion == id && h.Estado == true))!;
        }

        public async Task<OperationResult> GetByPisoAsync(int idPiso)
        {
            try
            {
                if (idPiso <= 0)
                    return OperationResult.Failure("El ID del piso debe ser mayor que cero.");
                
                var habitaciones = await _context.Habitaciones
                    .Where(h => h.IdPiso == idPiso && h.Estado == true)
                    .ToListAsync();
                
                return OperationResult.Success(habitaciones, 
                    habitaciones.Any() ? "Habitaciones obtenidas correctamente" : $"No se encontraron habitaciones en el piso {idPiso}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener habitaciones por piso {idPiso}");
                return OperationResult.Failure($"Error al obtener habitaciones: {ex.Message}");
            }
        }
        
        public async Task<OperationResult> GetByCategoriaAsync(string categoria)
        {
            try
            {
                var habitaciones = await _context.Habitaciones
                    .Join(_context.Categorias, h => h.IdCategoria, c => c.IdCategoria, 
                        (h, c) => new { h, c })
                    .Where(hc => hc.c.Descripcion != null && 
                                 EF.Functions.Like(hc.c.Descripcion, $"%{categoria}%") && 
                                 hc.c.Estado == true)
                    .Select(hc => hc.h)
                    .ToListAsync();

                return OperationResult.Success(habitaciones, 
                    habitaciones.Any() ? "Habitaciones obtenidas correctamente" : "No se encontraron habitaciones con la categoría especificada.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener habitaciones por categoría {categoria}");
                return OperationResult.Failure($"Error al obtener habitaciones: {ex.Message}");
            }
        }

        public async Task<OperationResult> GetInfoHabitacionesAsync()
        {
            try
            {
                var habitacionesQuery = from h in _context.Habitaciones
                    join p in _context.Pisos on h.IdPiso equals p.IdPiso
                    join c in _context.Categorias on h.IdCategoria equals c.IdCategoria
                    where h.Estado == true
                    select new
                    {
                        h.IdHabitacion,
                        h.Numero,
                        h.Detalle,
                        c.IdCategoria,
                        DescripcionPiso = p.Descripcion,
                        DescripcionCategoria = c.Descripcion, 
                        c.IdServicio
                    };

                var tarifasVigentes = await _context.Tarifas
                    .Where(t => t.FechaInicio <= DateTime.Now &&
                                t.FechaFin >= DateTime.Now &&
                                t.Estado == true)
                    .ToListAsync();

                var servicios = await _context.Set<Servicios>().ToListAsync();

                var habitacionesBase = await habitacionesQuery.ToListAsync();

                var habitacionesInfo = habitacionesBase
                    .GroupBy(h => h.IdHabitacion) 
                    .Select(group =>
                    {
                        var habitacion = group.First(); 

                        var tarifa = tarifasVigentes
                            .FirstOrDefault(t => t.IdCategoria == habitacion.IdCategoria);
                        var servicio = servicios
                            .FirstOrDefault(s => s.IdServicio == habitacion.IdServicio);

                        return new
                        {
                            habitacion.IdHabitacion,
                            habitacion.Numero,
                            habitacion.Detalle,
                            PrecioPorNoche = tarifa?.PrecioPorNoche ?? 0, 
                            habitacion.DescripcionPiso,
                            habitacion.DescripcionCategoria,
                            NombreServicio = servicio?.Nombre ?? "Sin servicio",
                            DescripcionServicio = servicio?.Descripcion ?? "Sin descripción"
                        };
                    })
                    .ToList();

                if (habitacionesInfo.Any())
                    return OperationResult.Success(habitacionesInfo);

                return OperationResult.Success(new { mensaje = "No se encontraron habitaciones con la información requerida" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener información de habitaciones");
                return OperationResult.Failure($"Error en la consulta: {ex.Message}");
            }
        }

        public async Task<OperationResult> GetByNumeroAsync(string numero)
        {
            try
            {
                var habitacion = await _context.Habitaciones
                    .FirstOrDefaultAsync(h => h.Numero == numero && h.Estado == true);
                
                return OperationResult.Success(habitacion, 
                    habitacion != null ? "Habitación encontrada" : $"No se encontró la habitación con número '{numero}'.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener habitación por número {numero}");
                return OperationResult.Failure($"Error al obtener habitación: {ex.Message}");
            }
        }

        public override async Task<OperationResult> SaveEntityAsync(Habitacion habitacion)
        {
            try
            {
                await _context.Habitaciones.AddAsync(habitacion);
                await _context.SaveChangesAsync();
                return OperationResult.Success(habitacion, "Habitación guardada exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar habitación");
                return OperationResult.Failure($"Error al guardar habitación: {ex.Message}");
            }
        }
        
        public override async Task<OperationResult> UpdateEntityAsync(Habitacion habitacion)
        {
            try
            {
                var existingRoom = await _context.Habitaciones.FindAsync(habitacion.IdHabitacion);
                if (existingRoom == null)
                    return OperationResult.Failure("La habitación no existe.");

                existingRoom.Numero = habitacion.Numero;
                existingRoom.Detalle = habitacion.Detalle;
                existingRoom.Precio = habitacion.Precio;
                existingRoom.IdPiso = habitacion.IdPiso;
                existingRoom.IdCategoria = habitacion.IdCategoria;
                existingRoom.IdEstadoHabitacion = habitacion.IdEstadoHabitacion;
                existingRoom.Estado = habitacion.Estado;
                
                await _context.SaveChangesAsync();

                return OperationResult.Success(existingRoom, "Habitación actualizada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar habitación {habitacion.IdHabitacion}");
                return OperationResult.Failure($"Error al actualizar habitación: {ex.Message}");
            }
        }
    }
}