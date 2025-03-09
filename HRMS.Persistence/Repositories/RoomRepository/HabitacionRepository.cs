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
            await OperationResult.ExecuteOperationAsync(async () =>
            {
                ValidateId(idPiso, "El ID del piso debe ser mayor que cero.");
                var habitaciones = await _context.Habitaciones.Where(h => h.IdPiso == idPiso 
                && h.Estado == true).ToListAsync();
                return habitaciones.Any() ? OperationResult.Success(habitaciones)
                    : OperationResult.Failure($"No se encontraron habitaciones en el piso {idPiso}.");
            });

   
        
        public async Task<OperationResult> GetByCategoriaAsync(string categoria) =>
            await OperationResult.ExecuteOperationAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(categoria)) return OperationResult.Failure("Debe ingresar una categoría.");

                var habitaciones = await _context.Habitaciones
                    .Join(_context.Categorias, h => h.IdCategoria, c => c.IdCategoria, 
                        (h, c) => new { h, c })
                    .Where(hc => hc.c.Descripcion != null && 
                                 EF.Functions.Like(hc.c.Descripcion, $"%{categoria}%") && 
                                 hc.c.Estado == true)
                    .Select(hc => hc.h)
                    .ToListAsync();

                return habitaciones.Any() ? OperationResult.Success(habitaciones) 
                    : OperationResult.Failure("No se encontraron habitaciones con la categoría especificada.");
            });

        public async Task<OperationResult> GetInfoHabitacionesAsync() =>
            await OperationResult.ExecuteOperationAsync(async () =>
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

                    return await HandleNoHabitacionesFound();
                }
                catch (Exception ex)
                {
                    return OperationResult.Failure($"Error en la consulta: {ex.Message}");
                }
            });

      public async Task<OperationResult> GetByNumeroAsync(string numero) =>
            await OperationResult.ExecuteOperationAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(numero)) return OperationResult.Failure("El número de habitación no puede estar vacío.");

                var habitacion = await _context.Habitaciones.FirstOrDefaultAsync(h => h.Numero == numero);
                return habitacion != null ? OperationResult.Success(habitacion) : 
                    OperationResult.Failure($"No se encontró la habitación con número '{numero}'.");
            });

      public override async Task<OperationResult> SaveEntityAsync(Habitacion habitacion) =>
          await OperationResult.ExecuteOperationAsync(async () =>
          {
              var validation = _validator.Validate(habitacion);
              if (!validation.IsSuccess) return validation;

              var foreignKeyValidation = await ValidateForeignKeys(habitacion);
              if (!foreignKeyValidation.IsSuccess) return foreignKeyValidation;
              
              await _context.Habitaciones.AddAsync(habitacion);
              await _context.SaveChangesAsync();

              return OperationResult.Success(habitacion, "Habitación guardada exitosamente.");
          });

      public override async Task<OperationResult> UpdateEntityAsync(Habitacion habitacion) =>
          await OperationResult.ExecuteOperationAsync(async () =>
          {
              var validation = _validator.Validate(habitacion);
              if (!validation.IsSuccess) return validation;

              var existingRoom = await _context.Habitaciones.FindAsync(habitacion.IdHabitacion);
              if (existingRoom == null) return OperationResult.Failure("La habitación no existe.");

              var foreignKeyValidation = await ValidateForeignKeys(habitacion);
              if (!foreignKeyValidation.IsSuccess) return foreignKeyValidation;


              UpdateHabitacion(existingRoom, habitacion);
              await _context.SaveChangesAsync();

              return OperationResult.Success(existingRoom, "Habitación actualizada correctamente.");
          });
        
        public async Task<bool> ExisteNumeroHabitacion(string numero, int? idExcluir = null)
        {
            var query = _context.Habitaciones.AsQueryable();
    
            if (idExcluir.HasValue)
                query = query.Where(h => h.IdHabitacion != idExcluir.Value);
        
            return await query.AnyAsync(h => h.Numero == numero);
        }

        private async Task<OperationResult> ValidateForeignKeys(Habitacion habitacion)
        {
            if (!await _context.Pisos.AnyAsync(p => p.IdPiso == habitacion.IdPiso))
                return OperationResult.Failure($"El piso con ID {habitacion.IdPiso} no existe.");
        
            if (!await _context.Pisos.AnyAsync(p => p.IdPiso == habitacion.IdPiso && p.Estado == true))
                return OperationResult.Failure($"El piso con ID {habitacion.IdPiso} está inactivo.");

            if (!await _context.Categorias.AnyAsync(c => c.IdCategoria == habitacion.IdCategoria && c.Estado == true))
                return OperationResult.Failure($"La categoría con ID {habitacion.IdCategoria} no existe o está inactiva.");

            if (!await _context.EstadoHabitaciones.AnyAsync(e => e.IdEstadoHabitacion == habitacion.IdEstadoHabitacion && e.Estado == true))
                return OperationResult.Failure($"El estado de habitación con ID {habitacion.IdEstadoHabitacion} no existe o está inactivo.");

            return OperationResult.Success();
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
        
        private async Task<OperationResult> HandleNoHabitacionesFound()
        {
            var hasHabitaciones = await _context.Habitaciones.AnyAsync(h => h.Estado == true);
            if (!hasHabitaciones)
                return OperationResult.Success(new { mensaje = "No hay habitaciones activas en la base de datos" });

            var hasTarifasVigentes = await _context.Tarifas.AnyAsync(t 
                => t.FechaInicio <= DateTime.Now && t.FechaFin >= DateTime.Now);
            if (!hasTarifasVigentes)
                return OperationResult.Success(new { mensaje = "No hay tarifas vigentes para la fecha actual" });

            var categoriasConServicio = await _context.Categorias
                .Join(_context.Set<Servicios>(), c 
                    => c.IdServicio, s => s.IdServicio, (c, s) =>
                    new { c.IdCategoria, c.Descripcion, s.Nombre })
                .ToListAsync();

            if (!categoriasConServicio.Any())
                return OperationResult.Success(new { mensaje = "No hay categorías con servicios asociados" });

            var habitacionesSinFiltroFechas = await _context.Habitaciones
                .Where(h => h.Estado == true)
                .Take(5)
                .Select(h => new { h.IdHabitacion, h.Numero })
                .ToListAsync();

            return OperationResult.Success(new
            {
                mensaje = "No se encontraron habitaciones que cumplan con todos los criterios",
                categoriasConServicio,
                habitacionesSinFiltroFechas
            });
        }
    }
}
