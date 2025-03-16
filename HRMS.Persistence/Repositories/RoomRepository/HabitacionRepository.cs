using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
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
        private readonly IValidator<Habitacion> _validator;

        public HabitacionRepository(HRMSContext context, ILogger<HabitacionRepository> logger, IValidator<Habitacion> validator) 
            : base(context)
        {
            _logger = logger;
            _validator = validator;
        }

        public override async Task<List<Habitacion>> GetAllAsync()
        {
            _logger.LogInformation("Obteniendo todas las habitaciones activas");
            return await _context.Habitaciones
                .Where(h => h.Estado == true)
                .ToListAsync();
        }

        public override async Task<Habitacion> GetEntityByIdAsync(int id)
        {
            _logger.LogInformation($"Obteniendo habitación por ID: {id}");
            return (id == 0
                ? null
                : await _context.Habitaciones
                    .FirstOrDefaultAsync(h => h.IdHabitacion == id && h.Estado == true))!;
        }
        public async Task<OperationResult> GetByPisoAsync(int idPiso)
        {
            try
            {
                _logger.LogInformation($"Obteniendo habitaciones por piso: {idPiso}");

                var validateId = ValidateId(idPiso, "El ID de piso no puede ser menor o igual a cero.");
                if (!validateId.IsSuccess)
                    return validateId;
                
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
                _logger.LogInformation($"Obteniendo habitaciones por categoría: {categoria}");
                var validateString = ValidateString(categoria, "La categoría no puede ser nula o vacía.");
                if (!validateString.IsSuccess)
                    return validateString;
                
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
                var habitaciones = await _context.Habitaciones
                    .Where(h => h.Estado == true)
                    .ToListAsync();

                if (!habitaciones.Any())
                    return OperationResult.Success(new List<object>(), "No se encontraron habitaciones");

                var pisosDict = await _context.Pisos.ToDictionaryAsync(p => p.IdPiso, p => p);
                var categoriasDict = await _context.Categorias.ToDictionaryAsync(c => c.IdCategoria, c => c);
                var serviciosDict = await _context.Set<Servicios>().ToDictionaryAsync(s => (int)s.IdServicio, s => s);
                var tarifasVigentes = await _context.Tarifas
                    .Where(t => t.FechaInicio <= DateTime.Now && t.FechaFin >= DateTime.Now && t.Estado == true)
                    .ToListAsync();

                var resultados = habitaciones.Select(h =>
                {
                    pisosDict.TryGetValue(h.IdPiso ?? 0, out var piso);
                    categoriasDict.TryGetValue(h.IdCategoria ?? 0, out var categoria);

                    var tarifa = h.IdCategoria.HasValue
                        ? tarifasVigentes.FirstOrDefault(t => t.IdCategoria == h.IdCategoria.Value)
                        : null;

                    Servicios servicio = null;
                    if (categoria != null)
                        serviciosDict.TryGetValue((int)categoria.IdServicio, out servicio);

                    return (object)new
                    {
                        IdHabitacion = h.IdHabitacion,
                        Numero = h.Numero ?? string.Empty,
                        Detalle = h.Detalle ?? string.Empty,
                        PrecioPorNoche = tarifa?.PrecioPorNoche ?? h.Precio ?? 0m,
                        DescripcionPiso = piso?.Descripcion ?? string.Empty,
                        DescripcionCategoria = categoria?.Descripcion ?? string.Empty,
                        NombreServicio = servicio?.Nombre ?? "Sin servicio",
                        DescripcionServicio = servicio?.Descripcion ?? "Sin descripción"
                    };
                }).ToList();

                return OperationResult.Success(resultados, "Información de habitaciones obtenida correctamente");
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error en la consulta: {ex.Message}");
            }
        }

        public async Task<OperationResult> GetByNumeroAsync(string numero)
        {
            try
            {
                _logger.LogInformation($"Obteniendo habitación por número: {numero}");
                
                var validationResult = ValidateString(numero, "El número de habitación no puede ser nulo o vacío.");
                if (!validationResult.IsSuccess)
                    return validationResult;
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
                _logger.LogInformation("Guardando nueva habitación");
                var validationResult = _validator.Validate(habitacion);
                if (!validationResult.IsSuccess)
                {
                    _logger.LogWarning("Error de validación al guardar habitación: {Error}", validationResult.Message);
                    return OperationResult.Failure(validationResult.Message);
                }
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
                _logger.LogInformation($"Actualizando habitación con ID: {habitacion.IdHabitacion}");
                var validationResult = _validator.Validate(habitacion);
                
                if (!validationResult.IsSuccess)
                {
                    _logger.LogWarning("Error de validación al actualizar habitación: {Error}", validationResult.Message);
                    return OperationResult.Failure(validationResult.Message);
                }
                
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
        private static OperationResult ValidateString(string value, string message)
        {
            if (string.IsNullOrWhiteSpace(value))
                return OperationResult.Failure(message);
            return OperationResult.Success();
        }
        
        private static OperationResult ValidateId(int id, string message)
        {
            if (id <= 0)
                return OperationResult.Failure(message);
            return OperationResult.Success();
        }
    }
}