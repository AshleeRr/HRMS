using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Domain.Entities.Servicio;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HRMS.Persistence.Repositories.RoomRepository
{
    public class HabitacionRepository : BaseRepository<Habitacion, int>, IHabitacionRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IValidator<Habitacion> _validator;
        private readonly ILogger<HabitacionRepository> _logger;

        public HabitacionRepository(HRMSContext context, IConfiguration 
            configuration, IValidator<Habitacion> validator, ILogger<HabitacionRepository> logger) : base(context)
        {
            _configuration = configuration;
            _validator = validator;
            _logger = logger;
        }

        public override async Task<List<Habitacion>> GetAllAsync() =>
            await _context.Habitaciones.Where(h => h.Estado == true).ToListAsync();

        public override async Task<Habitacion?> GetEntityByIdAsync(int id) =>
            id > 0 ? await _context.Set<Habitacion>().FindAsync(id) : null;

        public async Task<OperationResult> GetByPisoAsync(int idPiso) =>
            await ExecuteOperationAsync(async () =>
            {
                ValidateId(idPiso, "El ID del piso debe ser mayor que cero.");
                var habitaciones = await _context.Habitaciones.Where(h => h.IdPiso == idPiso).ToListAsync();
                return habitaciones.Any() ? Success(habitaciones)
                    : Failure($"No se encontraron habitaciones en el piso {idPiso}.");
            });

        public async Task<OperationResult> GetByCategoriaAsync(string categoria) =>
            await ExecuteOperationAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(categoria)) return Failure("Debe ingresar una categoría.");

                var habitaciones = await _context.Habitaciones
                    .Join(_context.Categorias, h => h.IdCategoria, c => c.IdCategoria, 
                        (h, c) => new { h, c })
                    .Where(hc => hc.c.Descripcion != null && hc.c.Descripcion.Contains
                        (categoria, StringComparison.OrdinalIgnoreCase) && hc.c.Estado == true)
                    .Select(hc => hc.h)
                    .ToListAsync();

                return habitaciones.Any() ? Success(habitaciones) 
                    : Failure("No se encontraron habitaciones con la categoría especificada.");
            });

        public async Task<OperationResult> GetInfoHabitacionesAsync() =>
            await ExecuteOperationAsync(async () =>
            {
                try
                {
                    var habitaciones = await (from h in _context.Habitaciones
                        join p in _context.Pisos on h.IdPiso equals p.IdPiso
                        join c in _context.Categorias on h.IdCategoria equals c.IdCategoria
                        join t in _context.Tarifas on c.IdCategoria equals t.IdCategoria
                        join s in _context.Set<Servicios>() on c.IdServicio equals s.IdServicio into serviciosGroup
                        from s in serviciosGroup.DefaultIfEmpty()
                        where h.Estado == true
                              && t.FechaInicio <= DateTime.Now 
                              && t.FechaFin >= DateTime.Now
                        select new
                        {
                            h.IdHabitacion,
                            h.Numero,
                            h.Detalle,
                            h.Estado,
                            t.PrecioPorNoche,
                            DescripcionPiso = p.Descripcion,
                            DescripcionCategoria = c.Descripcion,
                            NombreServicio = s != null ? s.Nombre : "Sin servicio",
                            DescripcionServicio = s != null ? s.Descripcion : "Sin descripción"
                        }).ToListAsync();

                    if (habitaciones.Any())
                        return Success(habitaciones);

                    return await HandleNoHabitacionesFound();
                }
                catch (Exception ex)
                {
                    return Failure($"Error en la consulta: {ex.Message}");
                }
            });
        public async Task<OperationResult> GetByNumeroAsync(string numero) =>
            await ExecuteOperationAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(numero)) return Failure("El número de habitación no puede estar vacío.");

                var habitacion = await _context.Habitaciones.FirstOrDefaultAsync(h => h.Numero == numero);
                return habitacion != null ? Success(habitacion) : 
                    Failure($"No se encontró la habitación con número '{numero}'.");
            });

        public override async Task<OperationResult> SaveEntityAsync(Habitacion habitacion) =>
            await ExecuteOperationAsync(async () =>
            {
                var validation = ValidateHabitacion(habitacion);
                if (!validation.IsSuccess) return validation;

                var foreignKeyValidation = await ValidateForeignKeys(habitacion);
                if (!foreignKeyValidation.IsSuccess) return foreignKeyValidation;

                var uniqueNumberValidation = await ValidateUniqueRoomNumber(habitacion);
                if (!uniqueNumberValidation.IsSuccess) return uniqueNumberValidation;

                await _context.Habitaciones.AddAsync(habitacion);
                await _context.SaveChangesAsync();

                return Success(habitacion, "Habitación guardada exitosamente.");
            });

        public override async Task<OperationResult> UpdateEntityAsync(Habitacion habitacion) =>
            await ExecuteOperationAsync(async () =>
            {
                var validation = ValidateHabitacion(habitacion);
                if (!validation.IsSuccess) return validation;

                var existingRoom = await _context.Habitaciones.FindAsync(habitacion.IdHabitacion);
                if (existingRoom == null) return Failure("La habitación no existe.");

                var foreignKeyValidation = await ValidateForeignKeys(habitacion);
                if (!foreignKeyValidation.IsSuccess) return foreignKeyValidation;

                var uniqueNumberValidation = await ValidateUniqueRoomNumber(habitacion, true);
                if (!uniqueNumberValidation.IsSuccess) return uniqueNumberValidation;

                UpdateHabitacion(existingRoom, habitacion);
                await _context.SaveChangesAsync();

                return Success(existingRoom, "Habitación actualizada correctamente.");
            });

        public async Task<bool> ExistenHabitacionesConEstadoAsync(int idEstado)
        {
            try
            {
                return await _context.Habitaciones
                    .AnyAsync(h => h.IdEstadoHabitacion == idEstado && h.Estado == true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar si existen habitaciones con el estado {IdEstado}", idEstado);
                return false;
            }
        }
        public async Task<OperationResult> GetByCategoriaIdAsync(int idCategoria) =>
            await ExecuteOperationAsync(async () =>
            {
                ValidateId(idCategoria, "El ID de la categoría debe ser mayor que cero.");
        
                var habitaciones = await _context.Habitaciones
                    .Where(h => h.IdCategoria == idCategoria && h.Estado == true)
                    .ToListAsync();
            
                return habitaciones.Any() 
                    ? Success(habitaciones, "Habitaciones encontradas para la categoría especificada.") 
                    : Failure($"No se encontraron habitaciones activas para la categoría con ID {idCategoria}.");
            });
        
        private async Task<OperationResult> ValidateUniqueRoomNumber(Habitacion habitacion, bool isUpdate = false)
        {
            if (isUpdate)
            {
                var existingRoom = await _context.Habitaciones.FindAsync(habitacion.IdHabitacion);
                if (existingRoom != null && habitacion.Numero != existingRoom.Numero)
                {
                    if (await ExistsAsync(h => h.Numero == habitacion.Numero && h.IdHabitacion != habitacion.IdHabitacion))
                        return Failure($"Ya existe otra habitación con el número '{habitacion.Numero}'. El número debe ser único en todos los pisos.");
                }
            }
            else
            {
                if (await ExistsAsync(h => h.Numero == habitacion.Numero))
                    return Failure($"Ya existe una habitación con el número '{habitacion.Numero}'. El número debe ser único en todos los pisos.");
            }
    
            return Success();
        }

        private async Task<OperationResult> ValidateForeignKeys(Habitacion habitacion)
        {
            if (!await _context.Pisos.AnyAsync(p => p.IdPiso == habitacion.IdPiso))
                return Failure($"El piso con ID {habitacion.IdPiso} no existe.");
        
            if (!await _context.Pisos.AnyAsync(p => p.IdPiso == habitacion.IdPiso && p.Estado == true))
                return Failure($"El piso con ID {habitacion.IdPiso} está inactivo.");

            if (!await _context.Categorias.AnyAsync(c => c.IdCategoria == habitacion.IdCategoria && c.Estado == true))
                return Failure($"La categoría con ID {habitacion.IdCategoria} no existe o está inactiva.");

            if (!await _context.EstadoHabitaciones.AnyAsync(e => e.IdEstadoHabitacion == habitacion.IdEstadoHabitacion && e.Estado == true))
                return Failure($"El estado de habitación con ID {habitacion.IdEstadoHabitacion} no existe o está inactivo.");

            return Success();
        }

        private static void UpdateHabitacion(Habitacion existing, Habitacion updated)
        {
            existing.Numero = updated.Numero;
            existing.Detalle = updated.Detalle;
            existing.Precio = updated.Precio;
            existing.IdPiso = updated.IdPiso;
            existing.IdCategoria = updated.IdCategoria;
            existing.IdEstadoHabitacion = updated.IdEstadoHabitacion;
        }

        private OperationResult ValidateHabitacion(Habitacion habitacion) => _validator.Validate(habitacion);

        private static void ValidateId(int value, string message)
        {
            if (value <= 0) throw new ArgumentException(message);
        }

        private async Task<OperationResult> ExecuteOperationAsync(Func<Task<OperationResult>> operation)
        {
            try
            {
                return await operation();
            }
            catch (Exception ex)
            {
                return Failure($"Ocurrió un error: {ex.Message}");
            }
        }

        private static OperationResult Success(object data = null, string message = null) =>
            new() { IsSuccess = true, Data = data, Message = message };

        private static OperationResult Failure(string message) =>
            new() { IsSuccess = false, Message = message };
        
        private async Task<OperationResult> HandleNoHabitacionesFound()
        {
            var hasHabitaciones = await _context.Habitaciones.AnyAsync(h => h.Estado == true);
            if (!hasHabitaciones)
                return Success(new { mensaje = "No hay habitaciones activas en la base de datos" });

            var hasTarifasVigentes = await _context.Tarifas.AnyAsync(t 
                => t.FechaInicio <= DateTime.Now && t.FechaFin >= DateTime.Now);
            if (!hasTarifasVigentes)
                return Success(new { mensaje = "No hay tarifas vigentes para la fecha actual" });

            var categoriasConServicio = await _context.Categorias
                .Join(_context.Set<Servicios>(), c 
                    => c.IdServicio, s => s.IdServicio, (c, s) =>
                    new { c.IdCategoria, c.Descripcion, s.Nombre })
                .ToListAsync();

            if (!categoriasConServicio.Any())
                return Success(new { mensaje = "No hay categorías con servicios asociados" });

            var habitacionesSinFiltroFechas = await _context.Habitaciones
                .Where(h => h.Estado == true)
                .Take(5)
                .Select(h => new { h.IdHabitacion, h.Numero })
                .ToListAsync();

            return Success(new
            {
                mensaje = "No se encontraron habitaciones que cumplan con todos los criterios",
                categoriasConServicio,
                habitacionesSinFiltroFechas
            });
        }
    }
}
